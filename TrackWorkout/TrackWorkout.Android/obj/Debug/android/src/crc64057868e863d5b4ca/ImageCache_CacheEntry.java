package crc64057868e863d5b4ca;


public class ImageCache_CacheEntry
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("FormsCurvedBottomNavigation.ImageCache+CacheEntry, FormsCurvedBottomNavigation", ImageCache_CacheEntry.class, __md_methods);
	}


	public ImageCache_CacheEntry ()
	{
		super ();
		if (getClass () == ImageCache_CacheEntry.class)
			mono.android.TypeManager.Activate ("FormsCurvedBottomNavigation.ImageCache+CacheEntry, FormsCurvedBottomNavigation", "", this, new java.lang.Object[] {  });
	}

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
