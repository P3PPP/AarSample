using Android.App;
using Android.Widget;
using Android.OS;
using System;
using Android.Views;
using Android.Graphics;

namespace HelloAndroid
{
	[Activity (Label = "HelloAndroid", MainLauncher = true)]
	public class MainActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.Main);

			FindViewById<TextView> (Resource.Id.textPackageName)
				.Text = "PackageName : " + Application.PackageName;

			var color = new Color (new Random ().Next ());
			color.A = 255;
			FindViewById<View> (Resource.Id.viewColor)
				.SetBackgroundColor(color);
		}
	}
}


