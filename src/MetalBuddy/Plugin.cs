//!CompilerOption:AddRef:SlimDx.dll

using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ff14bot;
using ff14bot.AClasses;
using ff14bot.Behavior;
using ff14bot.Enums;
using ff14bot.Interfaces;
using ff14bot.Managers;
using ff14bot.NeoProfiles;
using ff14bot.Objects;
using MetalBuddy.etc;
using Vector3 = SlimDX.Vector3;
using NVector3 = Clio.Utilities.Vector3;
using ff14bot.Helpers;
using MetalBuddy.db;

namespace MetalBuddy
{
    public class Plugin : BotPlugin {

        public class Variables
        {
            public static Settings settings;
        }

        public class Constants
        {
            public static String NAME = "Metal's Active Gameplay Assistant";
            public static String ACRONYM = "MAGA";
        }

        private LocationManager locman = new LocationManager();
        SettingsWindow sw;
        private RenderForm _renderForm;

        public static void Log(Color color, String msg, params object[] vars)
        {
            string _out = "[" + Plugin.Constants.ACRONYM + "] " + string.Format(msg, vars);
            Console.WriteLine(_out);
            Logging.Write(System.Windows.Media.Color.FromRgb(color.R, color.G, color.B), _out);
        }

        public static void Log(String msg)
        {
            Log(Color.FromArgb(0, 255, 0), msg);
        }

        public override void OnButtonPress()
        {
            sw.updateSettings();
            sw.Show();
        }

        public override void Dispose() { }

        public override void OnPulse()
        {
        }

        public override string Author
        {
            get
            {
                return "MetalSVT";
            }
        }

        public override Version Version
        {
            get
            {
                return new Version(0, 0, 1);
            }
        }

        public override string Name
        {
            get
            {
                return Plugin.Constants.NAME;
            }
        }

        public override void OnInitialize()
        {
            // Only happens once.
            Variables.settings = new Settings();
            sw = new SettingsWindow();

            Log(Plugin.Constants.NAME + " initialized!");
        }

        public override void OnShutdown()
        {
            Task.Run(OnDisableAsync);
        }

        public override void OnEnabled()
        {
            Task.Factory.StartNew(RunRenderForm, TaskCreationOptions.LongRunning);
            TreeHooks.Instance.OnHooksCleared += OnHooksCleared;
        }

        public void RunRenderForm()
        {
            OverlayManager.Drawing += Drawing;
            IntPtr targetWindow = Core.Memory.Process.MainWindowHandle;
            _renderForm = new RenderForm(targetWindow);
            Application.Run(_renderForm);
        }

        public override void OnDisabled()
        {
            Task.Run(OnDisableAsync);
            TreeHooks.Instance.OnHooksCleared -= OnHooksCleared;
        }

        private async Task OnDisableAsync()
        {
            OverlayManager.Drawing -= Drawing;

            if (_renderForm == null)
                return;

            await _renderForm.ShutdownAsync();
        }

        private void OnHooksCleared(object sender, EventArgs args)
        {
            //TreeHooks.Instance.AddHook("TreeStart", _coroutine);
        }

        private void Drawing(DrawingContext ctx)
        {
            if ((Variables.settings.OnlyForeground && Imports.GetForegroundWindow() != Core.Memory.Process.MainWindowHandle) || QuestLogManager.InCutscene)
            {
                return;
            }

            GameObjectManager.Update();

            NVector3 mypos = Core.Me.Location;
            Vector3 vecStart = new Vector3(mypos.X, mypos.Y, mypos.Z);
            int myLevel = Core.Me.ClassLevel;
            String mainName = locman.GetLocationString(WorldManager.ZoneId);

            if(Variables.settings.EnableStats) // Statistics.
            {
                StringBuilder sb = new StringBuilder();
                StringBuilder alert = new StringBuilder();

                GameObject currentTarget = Core.Me.CurrentTarget;

                if (!Variables.settings.AlertObject.Equals(""))
                {
                    foreach (GameObject obj in GameObjectManager.GameObjects)
                    {
                        if (obj.Name.ToLower().Equals(Variables.settings.AlertObject.ToLower()) || (Variables.settings.AlertObject.Equals("*") && obj.Name.Length > 1))
                        {
                            String details = "";
                            if(Core.Me.IsFacing(obj))
                            {
                                details += "Facing!";
                            }
                            alert.AppendLine(obj.Name + ": " + Math.Floor(obj.Location.X) + " EW, " + Math.Floor(obj.Location.Y) + " ALTI, " + Math.Floor(obj.Location.Z) + " NS\n\tDistance: " + Math.Floor(Core.Me.Distance(obj.Location)) + " yalms " + details);
                        }
                    }
                }

                sb.AppendLine(mainName);
                sb.AppendLine(Math.Floor(Core.Me.Location.X) + " EW," + Math.Floor(Core.Me.Location.Y) + " ALTI," + Math.Floor(Core.Me.Location.Z) + " NS");

                if (currentTarget != null)
                {
                    sb.AppendLine("Current Target: " + currentTarget.Name + ", Distance: " +
                                  Math.Round(currentTarget.Distance(), 3));

                    sb.AppendLine("Can Attack: " + currentTarget.CanAttack.ToString());

                    NVector3 end = currentTarget.Location;
                    Vector3 vecEnd = new Vector3(end.X, end.Y, end.Z);

                    ctx.DrawLine(vecStart, vecEnd, Color.DeepSkyBlue);
                }
                else
                {
                    sb.AppendLine("");
                }

                sb.AppendLine();

                if (true)
                {
                    ctx.DrawOutlinedText(sb.ToString(), Variables.settings.OverlayXPos, Variables.settings.OverlayYPos, Color.FromArgb(0, 255, 0), Color.FromArgb(0, 0, 0));
                    ctx.DrawOutlinedText(alert.ToString(), Variables.settings.AlertXPos, Variables.settings.AlertYPos, Color.FromArgb(255, 0, 0), Color.FromArgb(0, 0, 0));
                }
            }

            if (Variables.settings.EnableOverlay && false) // Target self.
            {
                ctx.DrawOutlinedBox(Core.Me.Location.Convert() + new Vector3(0, 1, 0), new Vector3(0.1f),
                    Color.FromArgb(255, Color.Blue));
            }

            if (Variables.settings.EnableOverlay)
            {

                foreach (GameObject obj in GameObjectManager.GameObjects)
                {



                    //if (!obj.IsVisible)
                    //    continue;

                    //if (Variables.settings.EnableOverlay) // Only targetable?
                    //{
                    //    if (obj.Type != GameObjectType.EventObject)
                    //    {
                    //        if (!obj.IsTargetable)
                    //            continue;
                    //    }
                    //}



                    if (obj.Type == GameObjectType.Mount)
                        continue;

                    var name = obj.Name;
                    var vecCenter = obj.Location.Convert() + new Vector3(0, 1, 0);


                    //.Where(i => i.Type == GameObjectType.GatheringPoint || i.Type == GameObjectType.BattleNpc || i.Type == GameObjectType.EventObject || i.Type == GameObjectType.Treasure || i.Type == GameObjectType.Pc)



                    var color = Color.FromArgb(150, Color.Blue);

                    //some generic objects. If you want to add a specific object it should probably go here or in it's own block below this.
                    if ((obj.Type == GameObjectType.GatheringPoint || obj.Type == GameObjectType.EventObject || obj.Type == GameObjectType.Treasure) && false) // Etc Objects.
                    {
                        if (obj.Type == GameObjectType.GatheringPoint)
                            color = Color.FromArgb(150, Color.BlueViolet);
                        if (obj.Type == GameObjectType.EventObject)
                            color = Color.FromArgb(150, Color.Fuchsia);
                        if (obj.Type == GameObjectType.Treasure)
                            color = Color.SandyBrown;

                        if (Variables.settings.EnableOverlay && !string.IsNullOrEmpty(name))
                            ctx.Draw3DText(name, vecCenter);

                        if (Variables.settings.EnableOverlay)
                        {
                            ctx.DrawOutlinedBox(vecCenter, new Vector3(0.1f), Color.FromArgb(150, color));
                        }




                        //if (Variables.settings.DrawGameObjectLines)
                        //{
                        //    if (!Variables.settings.DrawGameObjectLinesLos || obj.InLineOfSight())
                        //        ctx.DrawLine(vecStart, vecCenter, Color.FromArgb(150, color));
                        //}





                    }

                    var u = obj as Character;
                    if (u != null && false) // Draw all players? Draw NPC's.
                    {


                        var playerOrPlayerOwned = (!u.IsNpc || u.SummonerObjectId != GameObjectManager.EmptyGameObject);
                        /*if (!Variables.settings.DrawPlayers && playerOrPlayerOwned)
                        {
                            continue;
                        }*/

                        var hostilityColor = Color.FromArgb(150, Color.Green);

                        var uStatusFlags = u.StatusFlags;
                        if (uStatusFlags.HasFlag(StatusFlags.Hostile))
                        {
                            hostilityColor = Color.FromArgb(150, Color.Red);

                            //if (Variables.settings.DrawAggroRangeCircles)
                            //    ctx.DrawCircle(vecCenter, u.MyAggroRange, 64,
                            //                   Color.FromArgb(75, Color.DeepSkyBlue));
                        }

                        if (uStatusFlags == StatusFlags.None)
                            hostilityColor = Color.FromArgb(150, Color.Yellow);

                        if (uStatusFlags.HasFlag(StatusFlags.Friend) || uStatusFlags.HasFlag(StatusFlags.PartyMember) || uStatusFlags.HasFlag(StatusFlags.AllianceMember))
                            hostilityColor = Color.FromArgb(150, Color.Green);




                        if (playerOrPlayerOwned)
                        {
                            /*if (Variables.settings.DrawPlayerNames)
                            {
                                ctx.Draw3DText(name, vecCenter);
                            }*/
                        }
                        else
                        {
                            /*if (Variables.settings.DrawUnitNames)
                            {
                                if (!string.IsNullOrEmpty(name) && obj.IsTargetable)
                                    ctx.Draw3DText(name, vecCenter);
                            }*/
                        }
                        ctx.DrawOutlinedBox(vecCenter, new Vector3(0.1f), Color.FromArgb(255, hostilityColor));
                    } else if(u != null && Variables.settings.ShowTargetablePlayers)
                    {
                        var playerOrPlayerOwned = (!u.IsNpc || u.SummonerObjectId != GameObjectManager.EmptyGameObject);
                        Boolean attackable = u.CanAttack;

                        if(playerOrPlayerOwned && attackable)
                        {
                            ctx.DrawOutlinedBox(vecCenter, new Vector3(0.1f), Color.FromArgb(255, Color.Red));
                        }
                    }
                }
            }
        }
    }
}
