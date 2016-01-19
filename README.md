# app-customers
Build status: <img src="https://www.bitrise.io/app/7210fb7015b6b6b6.svg?token=y5K_xmtDXKdEzUprMqbWTg" />
<img src="https://raw.githubusercontent.com/xamarin/app-customers/master/Screenshots/Customers_Screens.jpg" />

A simple Xamarin.Forms demo app with three primary screens:

* a list screen
* a read-only detail screen
* an editable detail screen

Includes integrations such as:

* getting directions
* making calls
* sending text messages
* email composition

## Google Maps API key (Android)
For Android, you'll need to obtain a Google Maps API key:
https://developer.xamarin.com/guides/android/platform_features/maps_and_location/maps/obtaining_a_google_maps_api_key/

Insert it in `~/Droid/Properties/AndroidManifest.xml`:

    <application android:label="Customers" android:theme="@style/CustomersTheme">\
      ...
      <meta-data android:name="com.google.android.geo.API_KEY" android:value="[YOUR API KEY HERE]" />
      ...
    </application>

## Screens
<img src="https://raw.githubusercontent.com/xamarin/app-customers/master/Screenshots/Customers_ListPage.png" width="600" />
<img src="https://raw.githubusercontent.com/xamarin/app-customers/master/Screenshots/Customers_DetailPage.png" width="600" />
<img src="https://raw.githubusercontent.com/xamarin/app-customers/master/Screenshots/Customers_EditPage.png" width="600" />
<img src="https://raw.githubusercontent.com/xamarin/app-customers/master/Screenshots/Customers_GetDirections.png" width="600" />

