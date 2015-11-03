using SharedSource.GeoLite.MaxMind;
using Sitecore.Analytics.Lookups;
using Sitecore.Configuration;
using Sitecore.Diagnostics;
using System;
using System.Web;
using System.Web.Hosting;

namespace SharedSource.GeoLite.LookupProvider
{
    public class GeoLiteProvider : LookupProviderBase
    {
        public override WhoIsInformation GetInformationByIp(string ip)
        {
            #region Variable Declarations

            const string ModuleName = "SharedSource.GeoLite";

            WhoIsInformation information = new WhoIsInformation();

            #endregion

            Assert.ArgumentNotNull(ip, "ip");

            string geoCityPath = Settings.GetSetting(ModuleName + ".DatabaseLocation.GeoLiteCity", "~/app_data/GeoLiteCity.dat");
            
            string vLogging = Settings.GetSetting(ModuleName + ".VerboseLogging", "false");
            bool vLoggingResult = false;
            bool verboseLogging = bool.TryParse(vLogging, out vLoggingResult);

            if (!vLoggingResult)
            {
                verboseLogging = false;
            }

            try
            {
                if (verboseLogging)
                {
                    Log.Info(ModuleName + ": Lookup Initialized", this);
                    Log.Info(ModuleName + ": IP Address: " + ip, this);
                    Log.Info(ModuleName + ": geoLitePath: " + geoCityPath, this);
                    Log.Info(ModuleName + ": full path: " + HostingEnvironment.MapPath(geoCityPath), this);
                }

                var lookUpService = new LookupService(HostingEnvironment.MapPath(geoCityPath), LookupService.GEOIP_STANDARD);

                var location = lookUpService.getLocation(ip);

                if (location != null)
                {
                    information.Country = location.countryCode ?? string.Empty;
                    if (verboseLogging)
                    {
                        Log.Info(ModuleName + ": information.Country = " + information.Country, this);
                    }

                    information.Region = location.regionName ?? string.Empty;
                    if (verboseLogging)
                    {
                        Log.Info(ModuleName + ": information.Region = " + information.Region, this);
                    }

                    information.City = location.city ?? string.Empty;
                    if (verboseLogging)
                    {
                        Log.Info(ModuleName + ": information.City = " + information.City, this);
                    }

                    information.PostalCode = location.postalCode ?? string.Empty;
                    if (verboseLogging)
                    {
                        Log.Info(ModuleName + ": information.PostalCode = " + information.PostalCode, this);
                    }

                    information.Latitude = (location.latitude == 0) ? string.Empty : location.latitude.ToString();
                    if (verboseLogging)
                    {
                        Log.Info(ModuleName + ": information.Latitude = " + ((location.latitude == 0) ? string.Empty : location.latitude.ToString()), this);
                    }

                    information.Longitude = (location.longitude == 0) ? string.Empty : location.longitude.ToString();
                    if (verboseLogging)
                    {
                        Log.Info(ModuleName + ": information.Longitude = " + ((location.longitude == 0) ? string.Empty : location.longitude.ToString()), this);
                    }

                    information.MetroCode = (location.metro_code <= 0) ? string.Empty : location.metro_code.ToString();
                    if (verboseLogging)
                    {
                        Log.Info(ModuleName + ": information.MetroCode = " + ((location.metro_code <= 0) ? string.Empty : location.metro_code.ToString()), this);
                    }

                    information.AreaCode = (location.area_code <= 0) ? string.Empty : location.area_code.ToString();
                    if (verboseLogging)
                    {
                        Log.Info(ModuleName + ": information.AreaCode = " + ((location.area_code <= 0) ? string.Empty : location.area_code.ToString()), this);
                    }
                }
                else
                {
                    Log.Info(ModuleName + ": IP location cannot be determined.  IP Address: " + ip, this);
                    information.BusinessName = Settings.GetSetting(ModuleName + ".NoLocationText", "Not Available");
                }

                if (verboseLogging)
                {
                    Log.Info(ModuleName + ": IP Lookup Complete", this);
                }

                return information;
            }
            catch (Exception ex)
            {
                Log.Error(ModuleName + ": Exception occurred.  Exception: " + ex.Message, this);
            }

            return null;
        }
    }
}