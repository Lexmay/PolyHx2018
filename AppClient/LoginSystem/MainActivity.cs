﻿using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Threading;
using Amazon;
using Amazon.CognitoSync.SyncManager;
using Amazon.CognitoSync;
using Amazon.Auth.AccessControlPolicy;
using Amazon.CognitoIdentity;
using Amazon.Lambda;
using Amazon.Lambda.Model;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LoginSystem
{
    [Activity(Label = "LoginSystem", MainLauncher = false)]
    public class MainActivity : Activity
    {
        private Button mBtnSignUp;
        private ProgressBar mProgressBar;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // getting credentials
            CognitoAWSCredentials credentials = new CognitoAWSCredentials(
                "us-east-1:a205f4ac-4f87-49d6-b913-911e4eccb852",    // Cognito Identity Pool ID
                RegionEndpoint.USEast1 // Region
            );

            // creating syncmanager
            CognitoSyncManager syncManager = new CognitoSyncManager(
            credentials,
            new AmazonCognitoSyncConfig
            {
            RegionEndpoint = RegionEndpoint.USEast1 // Region
            });

            // create local dataset
            Dataset dataset = syncManager.OpenOrCreateDataset("omniwallet_user");

            //Read in Dataset Do not use here, use where you need to read
            string myValue = dataset.Get("myKey");

            // Create a record in a dataset and synchronize with the server  Do not use here, use it when you need to save in cloud Database
            dataset.OnSyncSuccess += SyncSuccessCallback;
            dataset.Put("myKey", "myValue");
            dataset.SynchronizeAsync();



            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.MenuListCategory);


            mBtnSignUp = FindViewById<Button>(Resource.Id.btnSignUp);
            mProgressBar = FindViewById<ProgressBar>(Resource.Id.progressBar1);

            mBtnSignUp.Click += async (object sender, EventArgs args) =>
                {
                    //Pull up dialog
                    FragmentTransaction transaction = FragmentManager.BeginTransaction();
                    dialog_SignUp signUpDialog = new dialog_SignUp();
                    signUpDialog.Show(transaction, "dialog fragment");

                    signUpDialog.mOnSignUpComplete += signUpDialog_mOnSignUpComplete;

                    AmazonLambdaClient client = new AmazonLambdaClient("AKIAJNCHGN3K4RPYJZ2A", "erXMlPHJ+8n6lQjlPWJNfF0xAnChWluM9MGJtd5y", RegionEndpoint.USEast1);

                    InvokeRequest ir = new InvokeRequest
                    {
                        FunctionName = "Test_2",
                        InvocationType = InvocationType.RequestResponse,
                        Payload = "\"tous des lettres majuscules\""
                    };

                    InvokeResponse response = await client.InvokeAsync(ir);

                    var sr = new StreamReader(response.Payload);
                    JsonReader reader = new JsonTextReader(sr);

                    var serilizer = new JsonSerializer();
                    var op = serilizer.Deserialize(reader);

                    Console.WriteLine(op);
                    Console.ReadLine();
                };         
        }

        void signUpDialog_mOnSignUpComplete(object sender, OnSignUpEventArgs e)
        {
            mProgressBar.Visibility = ViewStates.Visible;
            Thread thread = new Thread(ActLikeARequest);
            thread.Start();
    

        }
       
        private void ActLikeARequest()
        {
            RunOnUiThread(() => { mProgressBar.Visibility = ViewStates.Invisible; });
            int x = Resource.Animation.slide_right;            
        }
        void SyncSuccessCallback(object sender, SyncSuccessEventArgs e)
        {
            // Your handler code here
        }
    }
}

