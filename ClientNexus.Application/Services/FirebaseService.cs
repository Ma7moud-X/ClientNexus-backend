//using FirebaseAdmin;
//using Google.Apis.Auth.OAuth2;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace ClientNexus.Application.Services
//{
//    public class FirebaseService
//    {

//        private static bool _isFirebaseInitialized = false;

//        public static void InitializeFirebase()
//        {
//            if (!_isFirebaseInitialized)
//            {
//                var credentialsPath = "../ClientNexus.Infrastructure/client-nex-firebase-adminsdk-fbsvc-8fdafe6794.json"; // Path to the .json file 
//                FirebaseApp.Create(new AppOptions()
//                {
//                    Credential = GoogleCredential.FromFile(credentialsPath)
//                });
//                _isFirebaseInitialized = true;
//            }
//        }
//    }

//}

