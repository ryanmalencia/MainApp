using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Widget;
using DataTypes;
using Microsoft.AspNet.SignalR.Client;
using System.Threading.Tasks;
using WebAPIClient.APICalls;

namespace App1
{
    [Activity(Label = "Machines", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        AgentCollection AllAgents;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);
            //async get initial data
            GetData();
            //start signalr connection
            StartDataHub();
        }

        /// <summary>
        /// Start signalr connection
        /// Setup actions based on messages
        /// </summary>
        private void StartDataHub()
        {
            IHubProxy _hub;
            string url = @"http://10.0.0.205:44444/";
            var connection = new HubConnection(url);
            _hub = connection.CreateHubProxy("InformationHub");
            connection.Start();
            _hub.On("updateRunning", x => SetRunning(x));
            _hub.On("updateIdle", x => SetIdle(x));
            _hub.On("updateDead", x => SetDead(x));
            _hub.On("addAgent", x => AddAgent(x));
        }

        private void AddAgent(string agent)
        {
            LinearLayout agents = FindViewById<LinearLayout>(Resource.Id.MachineLayout);
            if (AllAgents == null)
            {
                AllAgents = new AgentCollection();
                AllAgents.AddAgent(new Agent(agent));
            }
            else
            {
                AllAgents.AddAgent(new Agent(agent));
            }
            TextView view = new TextView(ApplicationContext);
            view.SetText(agent, null);
            view.Gravity = Android.Views.GravityFlags.CenterHorizontal;
            RunOnUiThread(() => agents.AddView(view));
        }

        private void SetRunning(string str)
        {
            LinearLayout agents = FindViewById<LinearLayout>(Resource.Id.MachineLayout);
            if (AllAgents != null)
            {
                int i;
                for(i=0; i< AllAgents.Agents.Count; i++)
                {
                    if(str == AllAgents.Agents[i].Name)
                    {
                        RunOnUiThread(() => agents.RemoveViewAt(i));
                        TextView view = new TextView(ApplicationContext);
                        view.SetText(str, null);
                        view.Gravity = Android.Views.GravityFlags.CenterHorizontal;
                        view.SetTextSize(Android.Util.ComplexUnitType.Sp, 25);
                        view.SetTextColor(Color.Green);
                        RunOnUiThread(() => agents.AddView(view,i));
                        break;
                    }
                }
            }
        }

        private void SetIdle(string str)
        {
            LinearLayout agents = FindViewById<LinearLayout>(Resource.Id.MachineLayout);
            if (AllAgents != null)
            {
                int i;
                for (i = 0; i < AllAgents.Agents.Count; i++)
                {
                    if (str == AllAgents.Agents[i].Name)
                    {
                        RunOnUiThread(() => agents.RemoveViewAt(i));
                        TextView view = new TextView(ApplicationContext);
                        view.SetText(str, null);
                        view.Gravity = Android.Views.GravityFlags.CenterHorizontal;
                        view.SetTextSize(Android.Util.ComplexUnitType.Sp, 25);
                        view.SetTextColor(Color.White);
                        RunOnUiThread(() => agents.AddView(view,i));
                        break;
                    }
                }
            }
        }

        private void SetDead(string str)
        {
            LinearLayout agents = FindViewById<LinearLayout>(Resource.Id.MachineLayout);
            if (AllAgents != null)
            {
                int i;
                for (i = 0; i < AllAgents.Agents.Count; i++)
                {
                    if (str == AllAgents.Agents[i].Name)
                    {
                        RunOnUiThread(() => agents.RemoveViewAt(i));
                        TextView view = new TextView(ApplicationContext);
                        view.SetText(str, null);
                        view.Gravity = Android.Views.GravityFlags.CenterHorizontal;
                        view.SetTextColor(Color.Red);
                        view.SetTextSize(Android.Util.ComplexUnitType.Sp, 25);
                        RunOnUiThread(() => agents.AddView(view,i));
                        break;
                    }
                }
            }
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
                view.Gravity = Android.Views.GravityFlags.CenterHorizontal;
                view.SetTextColor(Color.White);
                view.SetTextSize(Android.Util.ComplexUnitType.Sp, 25);
                agents.AddView(view);
            }
            AllAgents = data;
        }

        static AgentCollection GetMachData()
        {
            AgentCollection col = AgentAPI.GetAllAgents();
            return col;
        }
    }
}
