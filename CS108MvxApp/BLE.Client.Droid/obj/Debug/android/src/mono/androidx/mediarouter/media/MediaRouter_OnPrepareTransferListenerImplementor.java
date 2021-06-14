package mono.androidx.mediarouter.media;


public class MediaRouter_OnPrepareTransferListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		androidx.mediarouter.media.MediaRouter.OnPrepareTransferListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onPrepareTransfer:(Landroidx/mediarouter/media/MediaRouter$RouteInfo;Landroidx/mediarouter/media/MediaRouter$RouteInfo;)Lcom/google/common/util/concurrent/ListenableFuture;:GetOnPrepareTransfer_Landroidx_mediarouter_media_MediaRouter_RouteInfo_Landroidx_mediarouter_media_MediaRouter_RouteInfo_Handler:AndroidX.MediaRouter.Media.MediaRouter/IOnPrepareTransferListenerInvoker, Xamarin.AndroidX.MediaRouter\n" +
			"";
		mono.android.Runtime.register ("AndroidX.MediaRouter.Media.MediaRouter+IOnPrepareTransferListenerImplementor, Xamarin.AndroidX.MediaRouter", MediaRouter_OnPrepareTransferListenerImplementor.class, __md_methods);
	}


	public MediaRouter_OnPrepareTransferListenerImplementor ()
	{
		super ();
		if (getClass () == MediaRouter_OnPrepareTransferListenerImplementor.class)
			mono.android.TypeManager.Activate ("AndroidX.MediaRouter.Media.MediaRouter+IOnPrepareTransferListenerImplementor, Xamarin.AndroidX.MediaRouter", "", this, new java.lang.Object[] {  });
	}


	public com.google.common.util.concurrent.ListenableFuture onPrepareTransfer (androidx.mediarouter.media.MediaRouter.RouteInfo p0, androidx.mediarouter.media.MediaRouter.RouteInfo p1)
	{
		return n_onPrepareTransfer (p0, p1);
	}

	private native com.google.common.util.concurrent.ListenableFuture n_onPrepareTransfer (androidx.mediarouter.media.MediaRouter.RouteInfo p0, androidx.mediarouter.media.MediaRouter.RouteInfo p1);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
