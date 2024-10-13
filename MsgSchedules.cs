using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using static GameServer.Pool;

namespace GameServer.Game.MsgTournaments
{
    public class MsgSchedules
    {
        public static ITournament CurrentTournament;
        internal static MsgGuildWar GuildWar;
        internal static MsgEliteGuildWar EliteGuildWar;
        internal static MsgPoleDomination PoleDomination;
        internal static MsgClassicClanWar ClassicClanWar;
        internal static MsgPoleDominationBI PoleDominationBI;
        internal static MsgPoleDominationDC PoleDominationDC;
        internal static MsgPoleDominationPC PoleDominationPC;
        internal static MsgClassPKWar ClassPkWar;
        internal static MsgCouples CouplesPKWar;
        internal static MsgClanWar ClanWar;
        internal static MsgPkWar PkWar;
        internal static MsgArena Arena;
        private static bool ConfirmTime24()
        {
            return (DateTime.Now.Hour == 2 ||
                DateTime.Now.Hour == 4 ||
                DateTime.Now.Hour == 6 ||
                DateTime.Now.Hour == 8 ||
                DateTime.Now.Hour == 10 ||
                DateTime.Now.Hour == 12 ||
                DateTime.Now.Hour == 14 ||
                DateTime.Now.Hour == 16 ||
                DateTime.Now.Hour == 18 ||
                DateTime.Now.Hour == 20 ||
                DateTime.Now.Hour == 22 ||
                DateTime.Now.Hour == 24);
        }
        internal static void Create()
        {
            Tournaments.Add(TournamentType.QuizShow, new MsgQuizShow(TournamentType.QuizShow));
            GuildWar = new MsgGuildWar();
            Arena = new MsgArena();
            EliteGuildWar = new MsgEliteGuildWar();
            ClassicClanWar = new MsgClassicClanWar();
            ClassPkWar = new MsgClassPKWar(ProcesType.Dead);
            #region PoleDomination
            PoleDomination = new MsgPoleDomination();
            PoleDominationBI = new MsgPoleDominationBI();
            PoleDominationDC = new MsgPoleDominationDC();
            PoleDominationPC = new MsgPoleDominationPC();
            #endregion
            PkWar = new MsgPkWar();
            CouplesPKWar = new MsgCouples();
            MsgBroadcast.Create();
        }

        public static void SpawnLavaBeast(bool firstwork = false)
        {
            var Map = ServerMaps[2056];
            int Loc = Pool.GetRandom.Next(0, Pool.LavaBeast.Count);
            var spawnLoc = Pool.LavaBeast[Loc];
            LavaBeast.RemoveAt(Loc);
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                string msg = $"LavaBeast has spawned in FrozenGrotto6 at {spawnLoc.X},{spawnLoc.Y}! Hurry find it and kill it.";
                Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage(msg, "ALLUSERS", "Server", MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));
                Database.Server.AddMapMonster(stream, Map, 20055, (ushort)spawnLoc.X, (ushort)spawnLoc.Y, 1, 1, 1);
                if (!firstwork)
                    Console.WriteLine($"Spawned Lava Beast at {spawnLoc.X},{spawnLoc.Y}");
            }
            GC.Collect();
        }

        internal static void SendInvitation(string Name, string Prize, ushort X, ushort Y, ushort map, ushort DinamicID, int Seconds, Game.MsgServer.MsgStaticMessage.Messages messaj = Game.MsgServer.MsgStaticMessage.Messages.None)
        {
            string Message = " " + Name + " is about to begin! Will you join it?";
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();


                var packet = new Game.MsgServer.MsgMessage(Message + Prize, MsgServer.MsgMessage.MsgColor.yellow, MsgServer.MsgMessage.ChatMode.Center).GetArray(stream);
                foreach (var client in GamePoll.Values)
                {
                    if (!client.Player.OnMyOwnServer || client.IsConnectedInterServer())
                        continue;
                    client.Player.MessageBox(Message, new Action<Client.GameClient>(user => user.Teleport(X, Y, map, DinamicID)), null, Seconds, messaj);
                }
            }
        }
        internal static void SendInvitation2(string Name, ushort X, ushort Y, ushort map, ushort DinamicID, int Seconds, Game.MsgServer.MsgStaticMessage.Messages messaj = Game.MsgServer.MsgStaticMessage.Messages.None, byte minlevel = 1, byte maxlevel = 137)
        {
            string Message = " " + Name + " has spawned! Hurry and kill it.";
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                foreach (var client in GamePoll.Values)
                {
                    if (client.Player.Level >= minlevel && client.Player.Level <= maxlevel)
                    {
                        if (!client.Player.OnMyOwnServer || client.IsConnectedInterServer())
                            continue;
                        client.Player.MessageBox(Message, new Action<Client.GameClient>(user => user.Teleport(X, Y, map, DinamicID)), null, Seconds, messaj);
                    }
                }
            }
        }
        internal unsafe static void SendSysMesage(string Messaj, Game.MsgServer.MsgMessage.ChatMode ChatType = Game.MsgServer.MsgMessage.ChatMode.TopLeft
           , Game.MsgServer.MsgMessage.MsgColor color = Game.MsgServer.MsgMessage.MsgColor.red, bool SendScren = false)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                var packet = new Game.MsgServer.MsgMessage(Messaj, color, ChatType).GetArray(stream);
                foreach (var client in GamePoll.Values)
                    client.Send(packet);
            }
        }
        static List<string> SystemMsgs = new List<string>() {
           "Selling/trading cps outside the game will lead to your accounts banned forever.",
            "Join our discord group to be in touch with the community and suggest/report stuff.",
            "Administrators have [GM/PM] in their names,do not trust anyone else claiming to be a [GM/PM].",
            "Refer our server and gain rewards! (contact GM/PM).",
            "Thanks for supporting us! we will keep on working to provide the best for you!",
            "Check out Guide in TwinCity for information about the game.",
            "Sharing accounts is done at your own risk. You alone are responsible for your own accounts, Support will not be given on cases for shared accounts.",
            "Always treat the STAFF of TrinityConquer with the utmost respect. No insulting/cursing about them or the server.",
            "It's forbidden to advertise any other servers. Your account will be permanently banned without prior notice/warning, Repeated offenses will result in your IP Address being permanently banned.",
            "It's forbidden to abuse bugs or any kind of bug/glitch found in the game, If a player discovers a bug/glitch in the game, it must be reported in Facebook or to the first STAFF member you can find.",
            "It's forbidden to use Bots/Hacks/Cheats in-game. If you find any working Bots/Hacks/Cheats please report them to our STAFF.",
            "Mouse clickers are allowed as long as you're not away-from-keyboard. If you're found using any mouse clicker or macro while away you'll be botjailed.",
            "Only English is allowed in the world chat.",
            "Selling/Trading accounts/items/gold outside the game for real life currencies, for items in other servers or for any other exchange or just the attempt of doing so, will result in all your accounts being permanently banned."
        };
        public static bool WeeklyDrop = false;
        public static DateTime WaterLordStillTime = DateTime.Now;
        public static DateTime WaterLordTime = DateTime.Now;
        private static int NextBoss = 0;
        internal static int LavaBeastsCount = 0;
        public static bool TCBossInv = false, TCBossLaunched = false, SwordINV = false, TeratoINV = false, TeratoLaunched, SwordLaunched = false, HourlyBossInv = false, HourlyBossLaunched = false;
        internal static void CheckUp(DateTime clock)
        {
            DateTime Now64 = DateTime.Now;
            if (!FullLoading)
                return;
            try
            {
                #region SystemMsgs in game
                if (Now64.Minute % 10 == 0 && Now64.Second > 58)
                {
                    var rndMsg = SystemMsgs[Pool.GetRandom.Next(0, SystemMsgs.Count)];
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage(rndMsg, "ALLUSERS", "Server", MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                    }
                }
                #endregion

                if (Now64.Minute == 26 && Now64.Second <= 3)
                    NextBoss = Role.Core.Random.Next(0, 2);

                PkWar.CheckUp();
                CouplesPKWar.CheckUp();
                if (CurrentTournament == null)
                {
                    CurrentTournament = Tournaments[TournamentType.QuizShow];
                }
                CurrentTournament.CheckUp();

                #region Poles
                #region PoleDomination
                if ((Now64.Hour == 01 || Now64.Hour == 05 || Now64.Hour == 09 || Now64.Hour == 13 || Now64.Hour == 17 || Now64.Hour == 21) && Now64.Minute == 10)
                {
                    if (PoleDomination.Proces == ProcesType.Dead)
                        PoleDomination.Start();
                    if (PoleDomination.Proces == ProcesType.Idle)
                    {
                        if (Now64 > PoleDomination.StampRound)
                            PoleDomination.Began();
                    }
                    if (PoleDomination.Proces != ProcesType.Dead)
                    {
                        if (DateTime.Now > PoleDomination.StampShuffleScore)
                        {
                            PoleDomination.ShuffleGuildScores();
                        }
                    }

                    if (PoleDomination.SendInvitation == false && Now64.Minute == 10)
                    {
                        SendInvitation("ApeCity PoleDomination", "ConquerPoints", 576, 623, 1020, 0, 60, MsgServer.MsgStaticMessage.Messages.None);
                        PoleDomination.SendInvitation = true;
                        
                    }
                }
                if ((Now64.Hour == 01 || Now64.Hour == 05 || Now64.Hour == 09 || Now64.Hour == 13 || Now64.Hour == 17 || Now64.Hour == 21) && Now64.Minute == 13)
                {
                    if (PoleDomination.Proces == ProcesType.Alive || PoleDomination.Proces == ProcesType.Idle)
                        PoleDomination.CompleteEndGuildWar();
                }
                #endregion
                #region PoleDomination
                if ((Now64.Hour == 02 || Now64.Hour == 06 || Now64.Hour == 10 || Now64.Hour == 14 || Now64.Hour == 18 || Now64.Hour == 22) && Now64.Minute == 10)
                {
                    if (PoleDominationBI.Proces == ProcesType.Dead)
                        PoleDominationBI.Start();
                    if (PoleDominationBI.Proces == ProcesType.Idle)
                    {
                        if (Now64 > PoleDominationBI.StampRound)
                            PoleDominationBI.Began();
                    }
                    if (PoleDominationBI.Proces != ProcesType.Dead)
                    {
                        if (DateTime.Now > PoleDominationBI.StampShuffleScore)
                        {
                            PoleDominationBI.ShuffleGuildScores();
                        }
                    }

                    if (PoleDominationBI.SendInvitation == false && Now64.Minute == 10)
                    {
                        SendInvitation("BirdIland PoleDomination", "ConquerPoints", 718, 573, 1015, 0, 60, MsgServer.MsgStaticMessage.Messages.None);
                        PoleDominationBI.SendInvitation = true;
                    }
                }
                if ((Now64.Hour == 02 || Now64.Hour == 06 || Now64.Hour == 10 || Now64.Hour == 14 || Now64.Hour == 18 || Now64.Hour == 22) && Now64.Minute == 13)
                {
                    if (PoleDominationBI.Proces == ProcesType.Alive || PoleDominationBI.Proces == ProcesType.Idle)
                        PoleDominationBI.CompleteEndGuildWar();
                }
                #endregion
                #region PoleDomination
                if ((Now64.Hour == 03 || Now64.Hour == 07 || Now64.Hour == 11 || Now64.Hour == 15 || Now64.Hour == 19 || Now64.Hour == 23) && Now64.Minute == 10)
                {
                    if (PoleDominationDC.Proces == ProcesType.Dead)
                        PoleDominationDC.Start();
                    if (PoleDominationDC.Proces == ProcesType.Idle)
                    {
                        if (Now64 > PoleDominationDC.StampRound)
                            PoleDominationDC.Began();
                    }
                    if (PoleDominationDC.Proces != ProcesType.Dead)
                    {
                        if (DateTime.Now > PoleDominationDC.StampShuffleScore)
                        {
                            PoleDominationDC.ShuffleGuildScores();
                        }
                    }

                    if (PoleDominationDC.SendInvitation == false && Now64.Minute == 10)
                    {
                        SendInvitation("DesertCity PoleDomination", "ConquerPoints", 469, 657, 1000, 0, 60, MsgServer.MsgStaticMessage.Messages.None);
                        PoleDominationDC.SendInvitation = true;
                    }
                }
                if ((Now64.Hour == 03 || Now64.Hour == 07 || Now64.Hour == 11 || Now64.Hour == 15 || Now64.Hour == 19 || Now64.Hour == 23) && Now64.Minute == 13)
                {
                    if (PoleDominationDC.Proces == ProcesType.Alive || PoleDominationDC.Proces == ProcesType.Idle)
                        PoleDominationDC.CompleteEndGuildWar();
                }
              
                #endregion
                #region PoleDomination
                if ((Now64.Hour == 04 || Now64.Hour == 08 || Now64.Hour == 12 || Now64.Hour == 16 || Now64.Hour == 20 || Now64.Hour == 00) && Now64.Minute == 10)
                {
                    if (PoleDominationPC.Proces == ProcesType.Dead)
                        PoleDominationPC.Start();
                    if (PoleDominationPC.Proces == ProcesType.Idle)
                    {
                        if (Now64 > PoleDominationPC.StampRound)
                            PoleDominationPC.Began();
                    }
                    if (PoleDominationPC.Proces != ProcesType.Dead)
                    {
                        if (DateTime.Now > PoleDominationPC.StampShuffleScore)
                        {
                            PoleDominationPC.ShuffleGuildScores();
                        }
                    }

                    if (PoleDominationPC.SendInvitation == false && Now64.Minute == 10)
                    {
                        SendInvitation("PhoenixCastle PoleDomination", "ConquerPoints", 275, 288, 1011, 0, 60, MsgServer.MsgStaticMessage.Messages.None);
                        PoleDominationPC.SendInvitation = true;
                    }
                }
                if ((Now64.Hour == 04 || Now64.Hour == 08 || Now64.Hour == 12 || Now64.Hour == 16 || Now64.Hour == 20 || Now64.Hour == 00) && Now64.Minute == 13)

                {
                    if (PoleDominationPC.Proces == ProcesType.Alive || PoleDominationPC.Proces == ProcesType.Idle)
                        PoleDominationPC.CompleteEndGuildWar();
                }

                #endregion
                #endregion

                #region LavaBeasts
                //if (DateTime.Now > NextLavaBeast && LavaBeastsCount > 0)
                //{
                //    LavaBeastsCount--;
                //    if (LavaBeastsCount > 0)
                //        NextLavaBeast = DateTime.Now.AddMinutes(3);
                //    SpawnLavaBeast();
                //}
                #endregion

                #region Bosses
                #region LavaBeasts
                //if (DateTime.Now > NextLavaBeast && LavaBeastsCount > 0)
                //{
                //    LavaBeastsCount--;
                //    if (LavaBeastsCount > 0)
                //        NextLavaBeast = DateTime.Now.AddMinutes(3);
                //    SpawnLavaBeast();
                //}
                #endregion
                #region Boss
                if (Now64.Minute == 40 && Now64.Second < 1)
                    MobsHandler.Generate(IDMonster.Ganoderma);
                #endregion
                #endregion

                if (Now64.DayOfWeek == DayOfWeek.Sunday) // Each Sunday
                {
                    #region GuildWar
                    if (Now64.Hour < 19) // 19 PM End GuildWar
                    {
                        if (GuildWar.Proces == ProcesType.Dead)
                            GuildWar.Start();
                        if (GuildWar.Proces == ProcesType.Idle)
                        {
                            if (Now64 > GuildWar.StampRound)
                                GuildWar.Began();
                        }
                        if (GuildWar.Proces != ProcesType.Dead)
                        {
                            if (DateTime.Now > GuildWar.StampShuffleScore)
                            {
                                GuildWar.ShuffleGuildScores();
                            }
                        }
                        if (Now64.Hour == 18)
                        {
                            if (GuildWar.FlamesQuest.ActiveFlame10 == false)
                            {
                                SendSysMesage("The Flame Stone 9 is Active now. Light up the Flame Stone (62,59) near the Stone Pole in the Guild City.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                                GuildWar.FlamesQuest.ActiveFlame10 = true;
                            }
                        }
                        else if (GuildWar.SendInvitation == false && Now64.Hour == 18)
                        {
                            SendInvitation("GuildWar", "ConquerPoints, Prizes", 200, 254, 1038, 0, 60, MsgServer.MsgStaticMessage.Messages.None);
                            GuildWar.SendInvitation = true;
                        }

                    }
                    else
                    {
                        if (GuildWar.Proces == ProcesType.Alive || GuildWar.Proces == ProcesType.Idle)
                        {
                            if (!GuildWar.ManualStarted)
                            {
                                GuildWar.CompleteEndGuildWar();
                            }
                        }
                    }
                    #endregion
                }

                #region ClassicClanWar


                if (Now64.Hour >= 14 && Now64.Hour < 15)
                {
                    if (ClassicClanWar.Proces == ProcesType.Dead)
                        ClassicClanWar.Start();
                    if (ClassicClanWar.Proces == ProcesType.Idle)
                    {
                        if (Now64 > ClassicClanWar.StampRound)
                            ClassicClanWar.Began();
                    }
                    if (ClassicClanWar.Proces != ProcesType.Dead)
                    {
                        if (DateTime.Now > ClassicClanWar.StampShuffleScore)
                        {
                            ClassicClanWar.ShuffleGuildScores();
                        }
                    }

                    if (ClassicClanWar.SendInvitation == false && (Now64.Hour == 14) && Now64.Minute == 00 && Now64.Second >= 00)
                    {
                        SendInvitation("ClassicClanWar", "ConquerPoints", 424, 251, 1002, 0, 60, MsgServer.MsgStaticMessage.Messages.None);
                        ClassicClanWar.SendInvitation = true;
                    }
                }
                else
                {
                    if (ClassicClanWar.Proces == ProcesType.Alive || ClassicClanWar.Proces == ProcesType.Idle)
                        ClassicClanWar.CompleteEndGuildWar();
                }
                
                #endregion

                #region ClassPK // Monday 18:00 PM
                if (Now64.DayOfWeek == DayOfWeek.Monday)
                {
                    if (Now64.Hour == 18 && Now64.Minute == 0)
                    {
                        ClassPkWar.Start();
                    }
                    if (Now64.Hour == 18 && Now64.Minute >= 10)
                    {
                        foreach (var war in ClassPkWar.PkWars)
                            foreach (var map in war)
                            {
                                var players_in_map = GamePoll.Values.Where(e => e.Player.DynamicID == map.DinamicID && e.Player.Alive);
                                if (players_in_map.Count() == 1)
                                {
                                    var winner = players_in_map.SingleOrDefault();
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        map.GetMyReward(winner, stream);
                                    }
                                }
                            }
                    }
                }
                #endregion

                #region EliteGuildWar
                if (EliteGuildWar != null && EliteGuildWar.Proces != ProcesType.Alive && EliteGuildWar != null)
                {
                    if (Now64.Hour >= 20 && Now64.Hour < 21)
                    {
                        if (EliteGuildWar.Proces == ProcesType.Dead)
                            EliteGuildWar.Start();
                        if (EliteGuildWar.Proces == ProcesType.Idle)
                        {
                            if (Now64 > EliteGuildWar.StampRound)
                                EliteGuildWar.Began();
                        }
                        if (EliteGuildWar.Proces != ProcesType.Dead)
                        {
                            if (DateTime.Now > EliteGuildWar.StampShuffleScore)
                            {
                                EliteGuildWar.ShuffleGuildScores();
                            }
                        }
                        if (EliteGuildWar.SendInvitation == false && (Now64.Hour == 20) && Now64.Minute == 30 && Now64.Second >= 30)
                        {
                            SendInvitation("EliteGuildWar", "ConquerPoints", 437, 249, 1002, 0, 60, MsgServer.MsgStaticMessage.Messages.None);
                            EliteGuildWar.SendInvitation = true;
                        }
                    }
                    else
                    {
                        if (EliteGuildWar.Proces == ProcesType.Alive || EliteGuildWar.Proces == ProcesType.Idle)
                            EliteGuildWar.CompleteEndGuildWar();
                    }
                }
                #endregion
            }
            catch (Exception e)
            {
                Console.SaveException(e);
            }
        }
    }
}
