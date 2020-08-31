using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using DragonsClaw.Models;

namespace DragonsClaw
{
    public class DataLayer
    {
        public static string GetGender(int gender)
        {
            switch (gender)
            {
                case 0:
                    return "Male";
                case 1:
                    return "Female";
                default:
                    return "Non-Binary";
            }
        }

        internal static EmpireViewModel GetEmpire(string username)
        {
            EmpireViewModel model = new EmpireViewModel
            {
                EmpireId = -1
            };

            PlayerViewModel player = DataLayer.GetPlayer(username);
            int hostId = -1;

            try
            {
                using (SqlConnection connection = new SqlConnection())
                {
                    connection.ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                    connection.Open();

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "SELECT * FROM Empires WHERE EmpireId = @EmpireId";

                        int empireId = DataLayer.GetEmpireId(player.PlayerId);

                        SqlParameter p = new SqlParameter
                        {
                            ParameterName = "@EmpireId",
                            Value = empireId
                        };

                        command.Parameters.Add(p);

                        var reader = command.ExecuteReader();

                        if (reader.HasRows)
                        {
                            reader.Read();

                            hostId = reader.GetInt32(3);

                            model.EmpireId = reader.GetInt32(0);
                            model.EmpireName = reader.GetString(1);
                            model.EmpireDescription = reader.GetString(2);
                            model.Host = DataLayer.GetPlayer(reader.GetInt32(3)).PlayerName;
                            model.ViceHost1 = reader.GetInt32(4);
                            model.ViceHost2 = reader.GetInt32(5);
                            model.Decree = reader.GetString(8);
                            model.Unimatrix = new System.Drawing.Point(
                                reader.GetInt32(6),
                                reader.GetInt32(7));
                        }

                        reader.Close();

                        command.CommandText = "SELECT * FROM EmpirePlayer AS e INNER JOIN " +
                            "Players AS p ON e.PlayerId = p.PlayerId WHERE e.EmpireId = @EmpireId";

                        command.Parameters.Clear();

                        p = new SqlParameter()
                        {
                            Value = empireId,
                            ParameterName = "@EmpireId"
                        };
                        command.Parameters.Add(p);
                        reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            EmpirePlayerViewModel q = new EmpirePlayerViewModel
                            {
                                Ruler = reader.GetString(5),
                                Sector = reader.GetString(6),
                                Race = GetRace(reader.GetInt32(19)),
                                Class = GetClass(reader.GetInt32(18)),
                                Planets = reader.GetInt32(8),
                                Networth = reader.GetInt32(32),
                                Role = Role.Member
                            };

                            int id = reader.GetInt32(1);

                            if (hostId == id)
                            {
                                q.Role = Role.Host;
                            }
                            else if (model.ViceHost1 == id || model.ViceHost2 == id)
                            {
                                q.Role = Role.ViceHost;
                            }

                            model.Players.Add(q);
                        }
                    }

                    connection.Close();
                }
            }
            catch (Exception exc)
            {
                LogMessage(exc.Message, "GetEmpire");
            }

            return model;
        }

        internal static bool CreateEmpire(CreateEmpireModel model, PlayerViewModel player)
        {
            bool success = false;

            try
            {
                using (SqlConnection connection = new SqlConnection())
                {
                    connection.ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                    connection.Open();

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText =
                            "INSERT INTO Empires (EmpireName, EmpireDescription, HostID, " +
                            "ViceHost1Id, ViceHost2Id, UnimatrixX, UnimatrixY, Decree) VALUES " +
                            "(@Name, '', @HostId, -1, -1, 0, 0, '')";

                        SqlParameter p = new SqlParameter
                        {
                            Value = model.EmpireName,
                            ParameterName = "@Name"
                        };

                        command.Parameters.Add(p);

                        p = new SqlParameter()
                        {
                            Value = player.PlayerId,
                            ParameterName = "@HostId"
                        };

                        command.Parameters.Add(p);
                        command.ExecuteNonQuery();

                        int empireId = GetEmpireIdByHost(player.PlayerId);

                        command.Parameters.Clear();
                        command.CommandText = "INSERT INTO EmpirePlayer (PlayerId, EmpireId) " +
                            "VALUES (@PlayerId, @EmpireId)";

                        p = new SqlParameter()
                        {
                            Value = player.PlayerId,
                            ParameterName = "@PlayerId"
                        };
                        
                        command.Parameters.Add(p);

                        p = new SqlParameter()
                        {
                            Value = empireId,
                            ParameterName = "@EmpireId"
                        };

                        command.Parameters.Add(p);
                        command.ExecuteNonQuery();
                        connection.Close();
                        return true;
                    }
                }
            }
            catch (Exception exc)
            {
                LogMessage(exc.Message, "CreateEmpire");
            }
            return success;
        }

        private static int GetEmpireId(int playerId)
        {
            int empireId = -1;

            try
            {
                using (SqlConnection connection = new SqlConnection())
                {
                    connection.ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                    connection.Open();

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "SELECT * FROM EmpirePlayer AS e INNER JOIN Players AS p ON e.PlayerId = p.PlayerId  WHERE p.PlayerId = @PlayerId";

                        SqlParameter p = new SqlParameter
                        {
                            ParameterName = "@PlayerId",
                            Value = playerId
                        };

                        command.Parameters.Add(p);

                        var reader = command.ExecuteReader();

                        if (reader.HasRows)
                        {
                            reader.Read();
                            empireId = reader.GetInt32(0);
                        }
                    }

                    connection.Close();
                }
            }
            catch (Exception exc)
            {
                LogMessage(exc.Message, "GetEmpireId");
            }

            return empireId;
        }

        private static int GetEmpireIdByHost(int playerId)
        {
            int empireId = -1;

            try
            {
                using (SqlConnection connection = new SqlConnection())
                {
                    connection.ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                    connection.Open();

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "SELECT * FROM Empires  WHERE HostId = @PlayerId";

                        SqlParameter p = new SqlParameter
                        {
                            ParameterName = "@PlayerId",
                            Value = playerId
                        };

                        command.Parameters.Add(p);

                        var reader = command.ExecuteReader();

                        if (reader.HasRows)
                        {
                            reader.Read();
                            empireId = reader.GetInt32(0);
                        }
                    }

                    connection.Close();
                }
            }
            catch (Exception exc)
            {
                LogMessage(exc.Message, "GetEmpireIdByHost");
            }

            return empireId;
        }

        internal static EmpireListViewModel GetEmpireList()
        {
            EmpireListViewModel model = new EmpireListViewModel();

            try
            {
                using (SqlConnection connection = new SqlConnection())
                {
                    connection.ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                    connection.Open();

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "SELECT * FROM Empires";

                        var reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            EmpireViewModel m = new EmpireViewModel
                            {
                                EmpireId = reader.GetInt32(0),
                                EmpireName = reader.GetString(1),
                                EmpireDescription = reader.GetString(2),
                                Host = DataLayer.GetPlayer(reader.GetInt32(3)).PlayerName,
                                ViceHost1 = reader.GetInt32(4),
                                ViceHost2 = reader.GetInt32(5),
                                Decree = reader.GetString(8),
                                Unimatrix = new System.Drawing.Point(
                                    reader.GetInt32(6),
                                    reader.GetInt32(7))
                            };

                            model.Empires.Add(m);
                        }
                    }

                    connection.Close();
                }
            }
            catch (Exception exc)
            {
                LogMessage(exc.Message, "GetEmpire");
            }

            return model;
        }

        internal static bool DoesPlayerExist(string username)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection())
                {
                    connection.ConnectionString = 
                        ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                    connection.Open();

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "SELECT * FROM PLAYERS WHERE UserEmail LIKE '" + username + "'";

                        var result = command.ExecuteReader();

                        if (result.HasRows)
                        {
                            connection.Close();
                            return true;
                        }
                    }

                    connection.Close();
                }
            }
            catch (Exception exc)
            {
                LogMessage(exc.Message, "CheckUserExists");
            }

            return false;
        }

        private static PlayerViewModel GetPlayer(int playerId)
        {
            PlayerViewModel model = new PlayerViewModel();

            try
            {
                using (SqlConnection connection = new SqlConnection())
                {
                    connection.ConnectionString =
                        ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                    connection.Open();

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "SELECT * FROM PLAYERS AS p WHERE PlayerId = " + playerId;

                        var reader = command.ExecuteReader();

                        if (reader.HasRows)
                        {
                            if (reader.Read())
                            {
                                model.PlayerId = reader.GetInt32(0);
                                model.PlayerName = reader.GetString(3);
                                model.SectorName = reader.GetString(4);
                                model.Credits = reader.GetInt32(5);
                                model.Food = reader.GetInt32(20);
                                model.Planets = reader.GetInt32(6);
                                model.PsionicEnergy = reader.GetInt32(7);
                                model.Population = reader.GetInt32(8);
                                model.Employed = reader.GetInt32(9);
                                model.Cadets = reader.GetInt32(10);
                                model.OffensiveTroops = reader.GetInt32(11);
                                model.DefensiveTroops = reader.GetInt32(12);
                                model.SpecialtyTroops = reader.GetInt32(13);
                                model.Mercenaries = reader.GetInt32(14);
                                model.Spies = reader.GetInt32(15);
                                model.ClassId = reader.GetInt32(16);
                                model.RaceId = reader.GetInt32(17);
                                model.Psionists = reader.GetInt32(18);
                                model.Gender = reader.GetInt32(19);
                                model.PsionicCrystals = reader.GetInt32(21);
                                model.Fighters = reader.GetInt32(22);
                                model.Bombers = reader.GetInt32(23);
                                model.Cruisers = reader.GetInt32(24);
                                model.Destroyers = reader.GetInt32(25);
                                model.Dreadnaughts = reader.GetInt32(26);
                                model.Raiders = reader.GetInt32(27);
                                model.Dilithium = reader.GetInt32(28);
                                model.Ore = reader.GetInt32(29);
                                model.Admirals = reader.GetInt32(30);
                                model.Observer = reader.GetBoolean(31);
                                model.NetWorth = reader.GetInt32(32);
                                model.NetWorthPerPlanet = reader.GetInt32(33);
                                model.Stealth = reader.GetInt32(34);
                                model.Terabytes = reader.GetInt32(35);
                                model.Happiness = reader.GetInt32(36);
                            }
                            connection.Close();
                            model.RaceName = GetRace(model.RaceId);
                            model.ClassName = GetClass(model.ClassId);
                            model.Exploring = GetExploring(model.PlayerId);
                            CalcNetWorth(model);
                            return model;
                        }
                    }

                    connection.Close();
                }
            }
            catch (Exception exc)
            {
                LogMessage(exc.Message, "GetPlayer");
            }

            return model;
        }

        internal static PlayerViewModel GetPlayer(string username)
        {
            PlayerViewModel model = new PlayerViewModel();

            try
            {
                using (SqlConnection connection = new SqlConnection())
                {
                    connection.ConnectionString = 
                        ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                    connection.Open();

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "SELECT * FROM PLAYERS AS p WHERE UserEmail LIKE '" + username + "%'";

                        var reader = command.ExecuteReader();

                        if (reader.HasRows)
                        {
                            if (reader.Read())
                            {
                                model.PlayerId = reader.GetInt32(0);
                                model.PlayerName = reader.GetString(3);
                                model.SectorName = reader.GetString(4);
                                model.Credits = reader.GetInt32(5);
                                model.Food = reader.GetInt32(20);
                                model.Planets = reader.GetInt32(6);
                                model.PsionicEnergy = reader.GetInt32(7);
                                model.Population = reader.GetInt32(8);
                                model.Employed = reader.GetInt32(9);
                                model.Cadets = reader.GetInt32(10);
                                model.OffensiveTroops = reader.GetInt32(11);
                                model.DefensiveTroops = reader.GetInt32(12);
                                model.SpecialtyTroops = reader.GetInt32(13);
                                model.Mercenaries = reader.GetInt32(14);
                                model.Spies = reader.GetInt32(15);
                                model.ClassId = reader.GetInt32(16);
                                model.RaceId = reader.GetInt32(17);
                                model.Psionists = reader.GetInt32(18);
                                model.Gender = reader.GetInt32(19);
                                model.PsionicCrystals = reader.GetInt32(21);
                                model.Fighters = reader.GetInt32(22);
                                model.Bombers = reader.GetInt32(23);
                                model.Cruisers = reader.GetInt32(24);
                                model.Destroyers = reader.GetInt32(25);
                                model.Dreadnaughts = reader.GetInt32(26);
                                model.Raiders = reader.GetInt32(27);
                                model.Dilithium = reader.GetInt32(28);
                                model.Ore = reader.GetInt32(29);
                                model.Admirals = reader.GetInt32(30);
                                model.Observer = reader.GetBoolean(31);
                                model.NetWorth = reader.GetInt32(32);
                                model.NetWorthPerPlanet = reader.GetInt32(33);
                                model.Stealth = reader.GetInt32(34);
                                model.Terabytes = reader.GetInt32(35);
                                model.Happiness = reader.GetInt32(36);
                            }
                            connection.Close();
                            model.RaceName = GetRace(model.RaceId);
                            model.ClassName = GetClass(model.ClassId);
                            model.Exploring = GetExploring(model.PlayerId);
                            CalcNetWorth(model);
                            return model;
                        }
                    }

                    connection.Close();
                }
            }
            catch (Exception exc)
            {
                LogMessage(exc.Message, "GetPlayer");
            }

            return model;
        }

        private static void CalcNetWorth(PlayerViewModel model)
        {
        }

        private static int GetExploring(int playerId)
        {
            return 0;
        }

        private static string GetClass(int classId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection())
                {
                    connection.ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                    connection.Open();

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "SELECT * FROM Classes WHERE ClassId = " + classId;

                        var result = command.ExecuteReader();

                        if (result.HasRows)
                        {
                            result.Read();
                            string Class = result.GetString(1);
                            connection.Close();
                            return Class;
                        }
                    }

                    connection.Close();
                }
            }
            catch (Exception exc)
            {
                LogMessage(exc.Message, "GetClass");
            }

            return string.Empty;
        }

        private static string GetRace(int raceId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection())
                {
                    connection.ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                    connection.Open();

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "SELECT * FROM Races WHERE RaceId = " + raceId;

                        var result = command.ExecuteReader();

                        if (result.HasRows)
                        {
                            result.Read();
                            string race = result.GetString(1);
                            connection.Close();
                            return race;
                        }
                    }

                    connection.Close();
                }
            }
            catch (Exception exc)
            {
                LogMessage(exc.Message, "GetRace");
            }

            return string.Empty;
        }

        private static void LogMessage(string message, string function)
        {

        }

        internal static void CreatePlayer(CreatePlayerModel model, string username)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection())
                {
                    connection.ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                    connection.Open();

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText =
                            "INSERT INTO Players (UserGuid, UserEmail, PlayerName, SectorName, Credits, Food, Planets, PsionicEnergy, PsionicCrystals," +
                            "Population, Employed, Cadets, OffensiveTroops, DefensiveTroops, SpecialtyTroops, " +
                            "Mercenaries, Spies, ClassId, RaceId, Psionists, Gender, Observer) VALUES (@UserGuid, " +
                            "@UserEmail, @PlayerName, @SectorName, 5000000, 5000, 500, 100, 0, 5000, 0, 5000, 0, 0, 0, " +
                            "0, 0, " + "@ClassId, @RaceId, 0, @Gender, @Observer)";

                        SqlParameter p = new SqlParameter
                        {
                            DbType = System.Data.DbType.String,
                            ParameterName = "@UserGuid",
                            Value = username
                        };

                        command.Parameters.Add(p);

                        p = new SqlParameter
                        {
                            DbType = System.Data.DbType.String,
                            ParameterName = "@UserEmail",
                            Value = username
                        };

                        command.Parameters.Add(p);

                        p = new SqlParameter
                        {
                            DbType = System.Data.DbType.AnsiString,
                            ParameterName = "@PlayerName",
                            Value = model.Name
                        };

                        command.Parameters.Add(p);

                        p = new SqlParameter
                        {
                            DbType = System.Data.DbType.AnsiString,
                            ParameterName = "@SectorName",
                            Value = model.SectorName
                        };

                        command.Parameters.Add(p);

                        p = new SqlParameter
                        {
                            DbType = System.Data.DbType.Int32,
                            ParameterName = "@RaceId",
                            Value = int.Parse(model.Race)
                        };

                        command.Parameters.Add(p);

                        p = new SqlParameter
                        {
                            DbType = System.Data.DbType.Int32,
                            ParameterName = "@ClassId",
                            Value = int.Parse(model.Class)
                        };

                        command.Parameters.Add(p);

                        p = new SqlParameter
                        {
                            DbType = System.Data.DbType.Int32,
                            ParameterName = "@Gender",
                            Value = int.Parse(model.Gender)
                        };

                        command.Parameters.Add(p);

                        p = new SqlParameter
                        {
                            ParameterName = "@Observer",
                            Value = model.Observer
                        };

                        command.Parameters.Add(p);
                        var result = command.ExecuteNonQuery();
                    }

                    connection.Close();
                }
            }
            catch (Exception exc)
            {
                //  todo: log the exception
                DataLayer.LogMessage(exc.Message, "CreatePlayer");
            }
        }
    }
}