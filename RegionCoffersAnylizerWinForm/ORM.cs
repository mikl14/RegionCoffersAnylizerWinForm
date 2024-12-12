using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RegionCoffersAnylizerWinForm.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RegionCoffersAnylizerWinForm
{

    internal static class ORM
    {
        static Dictionary<string, Models.Coffers> coffersDictionary = new Dictionary<string, Models.Coffers>();
        public static List<string> tablesNames = new List<string>();
        public static List<Filter> filters = new List<Filter>();
        public static HashSet<string> okveds = new HashSet<string>();

        public static DataService dataService;

        public static void clearMemory()
        {
            coffersDictionary.Clear();
            okveds.Clear();
        }

        public static void initTables(NalogiContext db)
        {
            tablesNames.Clear();
            FormattableString sql = FormattableStringFactory.Create("SELECT table_name FROM information_schema.tables WHERE table_schema = 'gs2024' AND table_type = 'BASE TABLE'");

            tablesNames = db.Database.SqlQuery<string>(sql).ToList();
        }

        public static void InitDatas(NalogiContext db, string tableName,string coffersTableName)
        {
            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;

            List<Models.Coffers> coffersList = new List<Models.Coffers>();
            Dictionary<string, Models.Coffers> coffersDictionary = new Dictionary<string, Models.Coffers>();
            List<Models.Region> regionsList = new List<Models.Region>();
            clearMemory();
            GC.Collect();

            FormattableString SqlRequest = FormattableStringFactory.Create("SELECT * FROM coffers2024." + coffersTableName + " WHERE inn IN (SELECT inn from gs2024."+ tableName+")");
            coffersList = db.Coffers.FromSql(SqlRequest).ToList();
            coffersDictionary = coffersList.GroupBy(x => x.Inn).ToDictionary(g => g.Key, g => g.FirstOrDefault());


            SqlRequest = FormattableStringFactory.Create("SELECT * FROM gs2024." + tableName + " WHERE inn IN (SELECT inn from coffers2024." + coffersTableName + ")");
            regionsList = (List<Models.Region>)db.Regions.FromSql(SqlRequest).ToList();

      

            okveds = getOkved(regionsList);


            string sql = $"SELECT LEFT(oktmof,8) as oktmof,COUNT(distinct inn) as count FROM gs2024.{tableName} group by LEFT(oktmof,8)";

            var res = db.Set<Models.CountRecord>().FromSqlRaw(sql).ToList();

            var dictionary = res.ToDictionary(g => g.oktmof, g => g.count);

            if (filters.Count > 0)
            {
                foreach (var filter in filters)
                {
                    regionsList = regionsList.Where(x => filter.isOperate(x)).ToList();
                }
            }

            dataService = new DataService(regionsList, coffersDictionary, dictionary);

            db.Database.CloseConnection();
        }

        public static HashSet<string> getOkved(List<Models.Region> reg)
        {
            HashSet<string> okveds = new HashSet<string>();

            foreach (var region in reg)
            {
                okveds.Add(region.FactOkvedOsn);
            }
            return okveds;
        }
      

        public static DataTable GetListDataTable(List<Models.Region> regions)
        {

            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("ИНН", typeof(string));
            dataTable.Columns.Add("Название/ФИО", typeof(string));
            dataTable.Columns.Add("ОКТМО МО", typeof(string));
            dataTable.Columns.Add("ОКТМО факт", typeof(string));
            dataTable.Columns.Add("ОКВЕД2 осн. факт.", typeof(string));
            dataTable.Columns.Add("Доля от России", typeof(string));
            dataTable.Columns.Add("Лицензия", typeof(string));

            foreach (var region in regions)
            {
                dataTable.Rows.Add(region.Inn,region.Naimobj, region.Oktmof.Substring(0,8) , region.Oktmof, region.FactOkvedOsn, dataService.coffersDictionary[region.Inn].Slice,region.License);
            }

            return dataTable;
        }

        public static DataTable[] getRegionDataTable()
        {
        
            tablesNames.Clear();

            Dictionary<string, Models.Region> MspPayers4 = new Dictionary<string, Models.Region>();
            Dictionary<string, Models.Region> NoMspPayers = new Dictionary<string, Models.Region>();


            DataTable[] dataTable = new DataTable[7];

            for(int i = 0; i < dataTable.Length;i++)
            {
                dataTable[i] = new DataTable();
            }

            dataTable[0].Columns.Add("OKTMO", typeof(string));
            dataTable[0].Columns.Add("Общее число субъектов", typeof(int));
            dataTable[0].Columns.Add("Число плательщиков", typeof(int));
            dataTable[0].Columns.Add("Плательщики МСП1", typeof(int));
            dataTable[0].Columns.Add("Плательщики МСП4", typeof(int));
            dataTable[0].Columns.Add("Плательщики НЕ МСП", typeof(int));
            dataTable[0].Columns.Add("Доля МСП1", typeof(decimal));
            dataTable[0].Columns.Add("Доля МСП4", typeof(decimal)) ;
            dataTable[0].Columns.Add("Доля НЕ МСП", typeof(decimal));
            dataTable[0].Columns.Add("Отобраные МСП 1", typeof(decimal));
            dataTable[0].Columns.Add("Отобраные МСП 4", typeof(decimal));
            dataTable[0].Columns.Add("Отобраные НЕ МСП", typeof(decimal));

            HashSet<Models.Region> findRegionInErmsp(string param, HashSet<Models.Region> regions)
            {
                HashSet<Models.Region> MspPayersResult = new HashSet<Models.Region>();

                    foreach (var region in regions)
                    {
                        if (region.Typeermsp == param || (param == "other" && region.Typeermsp != "1" && region.Typeermsp != "4"))
                        {
                                MspPayersResult.Add(region);
                        }
                    }
      
                return MspPayersResult;
            }

            List<Models.Region> persentsFullRegionsMsp1 = new List<Models.Region>();
            List<Models.Region> persentsFullRegionsMsp4 = new List<Models.Region>();
            List<Models.Region> persentsFullRegionsNoMsp = new List<Models.Region>();
            List<Models.Region> FullRegionsMsp1 = new List<Models.Region>();
            List<Models.Region> FullRegionsMsp4 = new List<Models.Region>();
            List<Models.Region> FullRegionsNoMsp = new List<Models.Region>();

            Dictionary<string, HashSet<Record>> oktmoGroupedMap = dataService.getRecordsByOktmo();

            foreach (var data in oktmoGroupedMap)
            {
               
                    HashSet<Models.Region> regionsMsp1 = findRegionInErmsp("1", dataService.getHashSetByPrimaryOktmo(data.Key));
                    HashSet<Models.Region> regionsMsp4 = findRegionInErmsp("4", dataService.getHashSetByPrimaryOktmo(data.Key));
                    HashSet<Models.Region> regionsNoMsp = findRegionInErmsp("other", dataService.getHashSetByPrimaryOktmo(data.Key));



                    List<Models.Region> persentsRegionsMsp1 = dataService.GetPersents(95, regionsMsp1.ToList());
                    List<Models.Region> persentsRegionsMsp4 = dataService.GetPersents(95, regionsMsp4.ToList());
                    List<Models.Region> persentsRegionsNoMsp = dataService.GetPersents(95, regionsNoMsp.ToList());



                    persentsFullRegionsMsp1.AddRange(persentsRegionsMsp1);
                    persentsFullRegionsMsp4.AddRange(persentsRegionsMsp4);
                    persentsFullRegionsNoMsp.AddRange(persentsRegionsNoMsp);

                    FullRegionsMsp1.AddRange(regionsMsp1);
                    FullRegionsMsp4.AddRange(regionsMsp4);
                    FullRegionsNoMsp.AddRange(regionsNoMsp);

                    dataTable[0].Rows.Add(data.Key, dataService.AllCountList[data.Key], data.Value.Count,
                        regionsMsp1.Count,
                        regionsMsp4.Count,
                        regionsNoMsp.Count,
                        dataService.getSliceFromList(regionsMsp1.ToList()),
                        dataService.getSliceFromList(regionsMsp4.ToList()),
                        dataService.getSliceFromList(regionsNoMsp.ToList()),      
                        persentsRegionsMsp1.Count,
                        persentsRegionsMsp4.Count,
                        persentsRegionsNoMsp.Count
                        );

                    oktmoGroupedMap.Remove(data.Key);     
            }

            dataTable[1] = GetListDataTable(persentsFullRegionsMsp1);
            dataTable[2] = GetListDataTable(persentsFullRegionsMsp4);
            dataTable[3] = GetListDataTable(persentsFullRegionsNoMsp);

            dataTable[4] = GetListDataTable(FullRegionsMsp1);
            dataTable[5] = GetListDataTable(FullRegionsMsp4);
            dataTable[6] = GetListDataTable(FullRegionsNoMsp);

            GC.Collect();

            return dataTable;
        }
    }
}
