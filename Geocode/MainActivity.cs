using Android.App;
using Android.Widget;
using Android.OS;
using Android.Locations;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Geocode
{
  [Activity(Label = "Geocode", MainLauncher = true, Icon = "@mipmap/icon")]
  public class MainActivity : Activity
  {
    ProgressDialog _progressDialog;

    protected override void OnCreate(Bundle savedInstanceState)
    {
      base.OnCreate(savedInstanceState);

      // Set our view from the "main" layout resource
      SetContentView(Resource.Layout.Main);

      _progressDialog = new ProgressDialog(this);
      _progressDialog.Indeterminate = true;
      _progressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);
      _progressDialog.SetMessage(GetString(Resource.String.geocodingProgress));
      _progressDialog.SetCancelable(false);

      Button geocodeButton = FindViewById<Button>(Resource.Id.geocodeButton);
      geocodeButton.Click += async (sender, e) => {
        await GeocodeAddress();
      };
    }

    async Task GeocodeAddress()
    {
      RunOnUiThread(() => _progressDialog.Show());

      string resultMessage;
      try 
      {
        var geocoder = new Geocoder(this);
        var results = await geocoder.GetFromLocationNameAsync(GetAddressText(), 1);
        System.Diagnostics.Debug.WriteLine("results: " + results);
        resultMessage = results != null && results.Count > 0 
          ? results[0].Latitude + ", " + results[0].Longitude
          : GetString(Resource.String.couldNotFindLocation);
      }
      catch (Exception ex)
      {
        System.Diagnostics.Debug.WriteLine(ex.StackTrace);
        resultMessage = ex.Message;
      }

      RunOnUiThread(() => {
        var resultText = FindViewById<TextView>(Resource.Id.resultText);
        resultText.Text = resultMessage;
        _progressDialog.Hide();
      });
    }

    string GetAddressText()
    {
      var addressText = FindViewById<EditText>(Resource.Id.addressText);
      if (String.IsNullOrEmpty(addressText.Text))
      {
        throw new Exception(GetString(Resource.String.pleaseProvideAddress));
      }
      return addressText.Text;
    }

  }
}


