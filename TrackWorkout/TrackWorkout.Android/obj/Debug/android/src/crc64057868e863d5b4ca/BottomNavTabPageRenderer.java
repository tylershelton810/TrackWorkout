package crc64057868e863d5b4ca;


public class BottomNavTabPageRenderer
	extends crc64720bb2db43a66fe9.TabbedPageRenderer
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onSizeChanged:(IIII)V:GetOnSizeChanged_IIIIHandler\n" +
			"n_onDraw:(Landroid/graphics/Canvas;)V:GetOnDraw_Landroid_graphics_Canvas_Handler\n" +
			"n_onLayout:(ZIIII)V:GetOnLayout_ZIIIIHandler\n" +
			"";
		mono.android.Runtime.register ("FormsCurvedBottomNavigation.BottomNavTabPageRenderer, FormsCurvedBottomNavigation", BottomNavTabPageRenderer.class, __md_methods);
	}


	public BottomNavTabPageRenderer (android.content.Context p0, android.util.AttributeSet p1, int p2)
	{
		super (p0, p1, p2);
		if (getClass () == BottomNavTabPageRenderer.class)
			mono.android.TypeManager.Activate ("FormsCurvedBottomNavigation.BottomNavTabPageRenderer, FormsCurvedBottomNavigation", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android:System.Int32, mscorlib", this, new java.lang.Object[] { p0, p1, p2 });
	}


	public BottomNavTabPageRenderer (android.content.Context p0, android.util.AttributeSet p1)
	{
		super (p0, p1);
		if (getClass () == BottomNavTabPageRenderer.class)
			mono.android.TypeManager.Activate ("FormsCurvedBottomNavigation.BottomNavTabPageRenderer, FormsCurvedBottomNavigation", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android", this, new java.lang.Object[] { p0, p1 });
	}


	public BottomNavTabPageRenderer (android.content.Context p0)
	{
		super (p0);
		if (getClass () == BottomNavTabPageRenderer.class)
			mono.android.TypeManager.Activate ("FormsCurvedBottomNavigation.BottomNavTabPageRenderer, FormsCurvedBottomNavigation", "Android.Content.Context, Mono.Android", this, new java.lang.Object[] { p0 });
	}


	public void onSizeChanged (int p0, int p1, int p2, int p3)
	{
		n_onSizeChanged (p0, p1, p2, p3);
	}

	private native void n_onSizeChanged (int p0, int p1, int p2, int p3);


	public void onDraw (android.graphics.Canvas p0)
	{
		n_onDraw (p0);
	}

	private native void n_onDraw (android.graphics.Canvas p0);


	public void onLayout (boolean p0, int p1, int p2, int p3, int p4)
	{
		n_onLayout (p0, p1, p2, p3, p4);
	}

	private native void n_onLayout (boolean p0, int p1, int p2, int p3, int p4);

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
