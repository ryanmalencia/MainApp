using System;
using System.Threading.Tasks;
using Android.App;
using Android.Widget;
using Android.OS;
using DataTypes;
using WebAPIClient.APICalls;

namespace App1
{
    [Activity(Label = "Phone Word", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);

            GetData();

            Button callButton = FindViewById<Button>(Resource.Id.CallButton);

            callButton.Click += (object sender, EventArgs e) =>
            {
                GetData();
            };
        }

        async void GetData()
        {
            AgentCollection data = await Task.Run(() => GetMachData());
            LinearLayout agents = FindViewById<LinearLayout>(Resource.Id.MachineLayout);
            agents.RemoveAllViews();
            foreach (Agent agent in data.Agents)
            {
                TextView view = new TextView(ApplicationContext);
                view.SetText(agent.Name, null);
                agents.AddView(view);
            }
        }

        static AgentCollection GetMachData()
        {
            AgentCollection col = AgentAPI.GetAllAgents();
            return col;
        }
    }
}

