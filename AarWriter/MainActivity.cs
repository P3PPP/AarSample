using Android.App;
using Android.Content;
using Android.Widget;
using Android.OS;
using Android.Nfc;
using Android.Nfc.Tech;
using System.Collections.Generic;

namespace AarWriter
{
	[Activity(Label = "AarWriter", MainLauncher = true)]
	[IntentFilter (new []{"android.nfc.action.NDEF_DISCOVERED"},
		Categories = new []{Intent.CategoryDefault},
		DataMimeType = "application/AarWriter.AarWriter")]
	public class MainActivity : Activity
	{
		private NfcAdapter nfcAdapter;
		private EditText editPackageName;
		private CheckBox checkMimeRecord;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.Main);

			nfcAdapter = NfcAdapter.GetDefaultAdapter(this);
			if (nfcAdapter == null)
			{
				FindViewById<TextView> (Resource.Id.textDescription).Text = 
					"This device can not use NFC.";
			}

			editPackageName = FindViewById<EditText> (Resource.Id.packageName);
			checkMimeRecord = FindViewById<CheckBox> (Resource.Id.checkDataMimeRecord);
		}

		protected override void OnNewIntent (Intent intent)
		{
			base.OnNewIntent (intent);

			Tag tag = intent.GetParcelableExtra (NfcAdapter.ExtraTag) as Tag;
			if (tag == null)
				return;

			if (editPackageName.Text == "")
				return;

			if (WriteNdefMessage(tag, CreateNdefMessage(editPackageName.Text)))
			{
				Toast.MakeText(this, "Completed.", ToastLength.Short).Show();
			}
			else
			{
				Toast.MakeText(this, "Failed.", ToastLength.Short).Show();
			}
		}

		protected override void OnResume()
		{
			base.OnResume();

			if (nfcAdapter != null)
			{
				var intent = new Intent(this, this.Class)
					.AddFlags(ActivityFlags.SingleTop | ActivityFlags.NewTask);
				var pendingIntent = PendingIntent.GetActivity(this, 0, intent, 0);
				nfcAdapter.EnableForegroundDispatch(this, pendingIntent, null, null);
			}
		}

		protected override void OnPause()
		{
			if (nfcAdapter != null)
			{
				nfcAdapter.DisableForegroundDispatch(this);
			}

			base.OnPause();
		}

		private NdefMessage CreateNdefMessage(string packageName)
		{
			var records = new List<NdefRecord>();
			records.Add(NdefRecord.CreateApplicationRecord(packageName));

			if (checkMimeRecord.Checked)
				records.Add(NdefRecord.CreateMime("application/" + packageName, null));

			return new NdefMessage(records.ToArray());
		}

		private static bool WriteNdefMessage(Tag tag, NdefMessage ndefMessage)
		{
			Ndef ndef = Ndef.Get (tag);
			if (ndef == null)
				return false;

			if (!ndef.IsWritable)
				return false;

			if (ndefMessage.ToByteArray().Length > ndef.MaxSize)
				return false;

			try
			{
				if(!ndef.IsConnected)
					ndef.Connect();

				ndef.WriteNdefMessage(ndefMessage);
				return true;
			}
			catch
			{
				return false;
			}
			finally
			{
				try
				{
					ndef.Close();
				}
				catch
				{
					// ignore.
				}
			}
		}
	}
}


