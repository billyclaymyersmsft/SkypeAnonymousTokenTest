using Microsoft.SfB.PlatformService.SDK.ClientModel;
using Microsoft.SfB.PlatformService.SDK.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkypeAnonTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, string> skypeAuthSettings = new Dictionary<string, string>()
                                                               {
                                                                   {"AAD_ClientId", ""},
                                                                   {"AAD_ClientSecret", ""},
                                                                   {"ApplicationEndpointId", ""},
                                                                   {"DiscoverUri", "https://api.skypeforbusiness.com/platformservice/discover?Region=northamerica"}
                                                               };

            var logger = new StringLogger();

            try
            {

                ClientPlatformSettings platformSettings =
                    new ClientPlatformSettings(new Uri(skypeAuthSettings["DiscoverUri"]), new Guid(skypeAuthSettings["AAD_ClientId"]), null, skypeAuthSettings["AAD_ClientSecret"], false);


                var platform = new ClientPlatform(platformSettings, logger);

                //Prepare endpoint
                var endpointSettings = new ApplicationEndpointSettings(new SipUri(skypeAuthSettings["ApplicationEndpointId"]));
                ApplicationEndpoint applicationEndpoint = new ApplicationEndpoint(platform, endpointSettings, null);

                var loggingContext = new LoggingContext(Guid.NewGuid());
                applicationEndpoint.InitializeAsync().GetAwaiter().GetResult();
                applicationEndpoint.InitializeApplicationAsync().GetAwaiter().GetResult();


                //Schedule meeting
                var input = new AdhocMeetingCreationInput(Guid.NewGuid().ToString("N") + "testMeeting");
                var adhocMeeting = applicationEndpoint.Application.CreateAdhocMeetingAsync(input, loggingContext).Result;

                Console.WriteLine("ad hoc meeting uri : " + adhocMeeting.OnlineMeetingUri);
                Console.WriteLine("ad hoc meeting join url : " + adhocMeeting.JoinUrl);

                //Get anon join token
                IAnonymousApplicationToken anonToken = applicationEndpoint.Application.GetAnonApplicationTokenForMeetingAsync(adhocMeeting.JoinUrl, "https://contoso.com;https://litware.com;http://www.microsoftstore.com/store/msusa/en_US/home<https://urldefense.proofpoint.com/v2/url?u=https-3A__contoso.com-3Bhttps-3A_litware.com-3Bhttp-3A_www.microsoftstore.com_store_msusa_en-5FUS_home&d=DwMGaQ&c=Bi8ZWNNcZUBhi-AHLorvrkVH0ArnzxTAZ7C8kNcJoZo&r=uycUjr279qo0kuj5jNiyFqi9adZMUkMtmsdo99NlvRQ&m=7jet88f1Tbswi73IIaQYzhITPDlxR_mzF2flXNxdbwQ&s=ozm8LVnKFoAyQtSX8KWnm56u950cmjrR5fvfEpP1SfM&e=>", Guid.NewGuid().ToString(), loggingContext).Result;

                Console.WriteLine("Get anon token : " + anonToken.AuthToken);
                Console.WriteLine("Get discover url for web SDK : " + anonToken.AnonymousApplicationsDiscoverUri);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.Read();
        }
    }
}
