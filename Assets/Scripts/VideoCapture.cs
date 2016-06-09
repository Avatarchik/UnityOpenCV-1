using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

public class VideoCapture : MonoBehaviour {
	// テクスチャのサイズ（横）
	public int width = 360;
	// テクスチャのサイズ（縦）
	public int height = 480;

	// テクスチャの描画先
	public Renderer renderTarget;

	// プラグインとの連携用
	// オブジェクトの生成
	[DllImport("__Internal")]
	private static extern IntPtr allocateVideoCapture(int width, int height);

	// オブジェクトの破棄
	[DllImport("__Internal")]
	private static extern void releaseVideoCapture(IntPtr capture);

	// 毎フレーム呼び出す用
	[DllImport("__Internal")]
	private static extern void updateVideoCapture(IntPtr capture, IntPtr image);

	// オブジェクト保持用
	private IntPtr nativeCapture;

	// テクスチャ周り
	private Texture2D texture;
	private Color32[] pixels;
	private GCHandle pixelsHandle;
	private IntPtr pixelsPtr;

	// Use this for initialization
	void Start () {
		#if UNITY_IOS
		nativeCapture = allocateVideoCapture(width, height);
		texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
		pixels = texture.GetPixels32();
		pixelsHandle = GCHandle.Alloc(pixels, GCHandleType.Pinned);
		pixelsPtr = pixelsHandle.AddrOfPinnedObject();
		#endif

		renderTarget.material.mainTexture = texture;
	}
	
	// Update is called once per frame
	void Update () {
		#if UNITY_IOS
		updateVideoCapture(nativeCapture, pixelsPtr);
		texture.SetPixels32(pixels);
		texture.Apply();
		#endif
	
	}

	void OnDestroy () {
		#if UNITY_IOS
		pixelsHandle.Free();
		releaseVideoCapture(nativeCapture);
		#endif
	}
}
