using SharedSource.GeoLite.MaxMind;
using Sitecore.Analytics.Lookups;
using Sitecore.Configuration;
using Sitecore.Diagnostics;
using System;
using System.Globalization;
using Sitecore;
using System.Web.Hosting;
using Sitecore.Analytics.Model;

namespace SharedSource.GeoLite.LookupProvider
{
    public class GeoLiteProvider : LookupProviderBase
    {
        [UsedImplicitly]
        public override WhoIsInformation GetInformationByIp(string ip)
        {
            #region Variable Declarations

            const string moduleName = "SharedSource.GeoLite";

            WhoIsInformation information = new WhoIsInformation();

            #endregion

            Assert.ArgumentNotNull(ip, "ip");

            string geoCityPath = Settings.GetSetting(moduleName + ".DatabaseLocation.GeoLiteCity", "~/app_data/GeoLiteCity.dat");
            
            string vLogging = Settings.GetSetting(moduleName + ".VerboseLogging", "false");
            bool vLoggingResult;
            bool verboseLogging = bool.TryParse(vLogging, out vLoggingResult);

            if (!vLoggingResult)
            {
                verboseLogging = false;
            }

            try
            {
                if (verboseLogging)
                {
                    Log.Info(moduleName + ": Lookup Initialized", this);
                    Log.Info(moduleName + ": IP Address: " + ip, this);
                    Log.Info(moduleName + ": geoLitePath: " + geoCityPath, this);
                    Log.Info(moduleName + ": full path: " + HostingEnvironment.MapPath(geoCityPath), this);
                }

                var lookUpService = new LookupService(HostingEnvironment.MapPath(geoCityPath), LookupService.GEOIP_STANDARD);

                var location = lookUpService.getLocation(ip);

                if (location != null)
                {
                    information.Country = location.countryCode ?? string.Empty;
                    if (verboseLogging)
                    {
                        Log.Info(moduleName + ": information.Country = " + information.Country, this);
                    }

                    information.Region = location.regionName ?? string.Empty;
                    if (verboseLogging)
                    {
                        Log.Info(moduleName + ": information.Region = " + information.Region, this);
                    }

                    information.City = location.city ?? string.Empty;
                    if (verboseLogging)
                    {
                        Log.Info(moduleName + ": information.City = " + information.City, this);
                    }

                    information.PostalCode = location.postalCode ?? string.Empty;
                    if (verboseLogging)
                    {
                        Log.Info(moduleName + ": information.PostalCode = " + information.PostalCode, this);
                    }

                    information.Latitude = location.latitude.Equals(0) ? 0 : location.latitude;
                    if (verboseLogging)
                    {
                        Log.Info(moduleName + ": information.Latitude = " + (location.latitude.Equals(0) ? string.Empty : location.latitude.ToString(CultureInfo.InvariantCulture)), this);
                    }

                    information.Longitude = location.longitude.Equals(0) ? 0 : location.longitude;
                    if (verboseLogging)
                    {
                        Log.Info(moduleName + ": information.Longitude = " + (location.longitude.Equals(0) ? string.Empty : location.longitude.ToString(CultureInfo.InvariantCulture)), this);
                    }

                    information.MetroCode = (location.metro_code <= 0) ? string.Empty : location.metro_code.ToString();
                    if (verboseLogging)
                    {
                        Log.Info(moduleName + ": information.MetroCode = " + ((location.metro_code <= 0) ? string.Empty : location.metro_code.ToString()), this);
                    }

                    information.AreaCode = (location.area_code <= 0) ? string.Empty : location.area_code.ToString();
                    if (verboseLogging)
                    {
                        Log.Info(moduleName + ": information.AreaCode = " + ((location.area_code <= 0) ? string.Empty : location.area_code.ToString()), this);
                    }
                }
                else
                {
                    Log.Info(moduleName + ": IP location cannot be determined.  IP Address: " + ip, this);
                    information.BusinessName = Settings.GetSetting(moduleName + ".NoLocationText", "Not Available");
                }

                if (verboseLogging)
                {
                    Log.Info(moduleName + ": IP Lookup Complete", this);
                }

                return information;
            }
            catch (Exception ex)
            {
                Log.Error(moduleName + ": Exception occurred.  Exception: " + ex.Message, this);
            }

            return null;
        }
    }
}