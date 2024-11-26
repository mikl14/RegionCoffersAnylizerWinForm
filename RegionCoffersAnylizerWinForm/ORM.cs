using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RegionCoffersAnylizerWinForm.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RegionCoffersAnylizerWinForm
{
    internal class ORM
    {
        public struct Record
        {
            public int countReg = 0;

            public HashSet<Models.Region> region;
            public Models.Coffers coffer;

            public Record(HashSet<Models.Region> region, Models.Coffers coffer) { this.region = region; this.coffer = coffer; }

            public void setReg(int reg) { this.countReg = reg; }
        }

        public List<Models.Region> regionsList =  new List<Models.Region>();

        public Dictionary<string, int> AllCountList = new Dictionary<string, int>();

        public List<Models.Coffers> coffersList = new List<Models.Coffers> ();

        public Dictionary<string,Record> dataDictionary = new Dictionary<string, Record>();
        Dictionary<string, HashSet<Models.Region>> regionsDictionary = new Dictionary<string, HashSet<Models.Region>>();

        Dictionary<string, Models.Coffers> coffersDictionary = new Dictionary<string, Models.Coffers>();

        public List<string> tablesNames = new List<string>();


        public void InitDatas(NalogiContext db, string tableName,string coffersTableName)
        {
            regionsList.Clear();
            AllCountList.Clear();
            coffersList.Clear();
            dataDictionary.Clear();
            regionsDictionary.Clear();
            coffersDictionary.Clear();
            tablesNames.Clear();


            
         Dictionary<string, List<Models.Region>> OktmoRegionsDictionary;


            FormattableString sql = FormattableStringFactory.Create("SELECT table_name FROM information_schema.tables WHERE table_schema = 'gs2024' AND table_type = 'BASE TABLE'");

            tablesNames = db.Database.SqlQuery<string>(sql).ToList();

            FormattableString SqlRequest = FormattableStringFactory.Create("SELECT * FROM coffers2024." + coffersTableName);
            coffersList = db.Coffers.FromSql(SqlRequest).ToList();
            coffersDictionary = coffersList.GroupBy(x => x.Inn).ToDictionary(g => g.Key, g => g.FirstOrDefault());


            SqlRequest = FormattableStringFactory.Create("SELECT * FROM gs2024." + tableName);
            regionsList = (List<Models.Region>)db.Regions.FromSql(SqlRequest).ToList();
            regionsDictionary = regionsList.Where(x => x.Inn != null) .GroupBy(x => x.Inn).ToDictionary(g => g.Key, g => g.ToHashSet());

          

            OktmoRegionsDictionary = regionsList.Where(x => x.Inn != null)
                .GroupBy(x => x.Oktmof.Substring(0,8)).ToDictionary(g => g.Key, g => g.ToList());


            foreach (var cofferEntry in coffersDictionary)
            {
                if (regionsDictionary.ContainsKey(cofferEntry.Value.Inn))
                {
                    dataDictionary.Add(cofferEntry.Key, new Record(regionsDictionary[cofferEntry.Value.Inn], cofferEntry.Value));
                }
                
            }

            foreach(var oktmoEntry in OktmoRegionsDictionary)
            {
                AllCountList.Add(oktmoEntry.Key, getCountDistinct(oktmoEntry.Value));
            }


        }

        public int getCountDistinct(List<Models.Region> reg)
        {
            HashSet<Models.Region> modelsReg = new HashSet<Models.Region>();

            foreach (var region in reg)
            {
                modelsReg.Add(region);
            }

            return modelsReg.Count;

        }
        public Dictionary<string, Record> regroupByOktmo(Dictionary<string, Record> dataDictionary)
        {
            Dictionary<string,Record> oktmoGroupedMap = new Dictionary<string,Record>();

            var noDataRegionsListDistict = regionsList.GroupBy(p => p.Inn).Select(g => g.First()).ToList();

            foreach (var dataEntry in dataDictionary)
            {
                foreach (var region in dataEntry.Value.region)
                {
                    string primaryOKTMO = region.Oktmof.Substring(0, 8);

                    if (!oktmoGroupedMap.ContainsKey(primaryOKTMO))
                    {
                        Record buf = new Record();

               

                        buf.coffer = dataEntry.Value.coffer;
                        buf.region = new HashSet<Models.Region>();
                        buf.region.Add(region);

                        oktmoGroupedMap.Add(primaryOKTMO, buf);
                    }
                    else
                    {
                      
                            oktmoGroupedMap[primaryOKTMO].region.Add(region);
                          
             
                    }
                }
            }

            return oktmoGroupedMap;
        }

        decimal getSliceFromList(List<Models.Region> regionList)
        {
            decimal result = 0;

            foreach (Models.Region region in regionList)
            {

                result += coffersDictionary[region.Inn].Slice;
            }
            return result;
        }

        public List<Models.Region> GetPersents(int persents, List<Models.Region> regions)
        {
            decimal sum = getSliceFromList(regions);
            decimal comulativeSum = 0;

            List<Models.Coffers> coffersBuf = new List<Models.Coffers>();

            List<Models.Region> result = new List<Models.Region>();

            if (regions.Count <= 4 )
            {
                return regions;
            }
            foreach (var region in regions)
            {
                coffersBuf.Add(coffersDictionary[region.Inn]);
            }

            coffersBuf.Sort((x, y) => y.Slice.CompareTo(x.Slice));

           

            
            foreach (var coffers in coffersBuf)
            {
                if (comulativeSum < (sum * (persents * (decimal)0.01)))
                {
                    result.Add(regions.Where(a => a.Inn == coffers.Inn).First());
                    comulativeSum += coffers.Slice;
                }
                else
                {
                    return result;
                }
            }
            return result;

        }

        public DataTable GetListDataTable(List<Models.Region> regions)
        {

            DataTable dataTable = new DataTable();

            dataTable.Columns.Add("Inn", typeof(string));

            foreach(var region in regions)
            {
                dataTable.Rows.Add(region.Inn);
            }

            return dataTable;
        }

        public DataTable[] getRegionDataTable()
        {

            Dictionary<string, Record> oktmoGroupedMap = regroupByOktmo(dataDictionary);

           
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


  


            HashSet<Models.Region> findRegionInMsp(string param, HashSet<Models.Region> regions)
            {
                HashSet<Models.Region> MspPayersResult = new HashSet<Models.Region>();

                    foreach (var region in regions)
                    {
                        if (region.TypeMsp == param || (param == "other" && region.TypeMsp != "1" && region.TypeMsp != "4"))
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

            foreach (var data in oktmoGroupedMap)
            {
                HashSet<Models.Region> regionsMsp1 = findRegionInMsp("1", data.Value.region);
                HashSet<Models.Region> regionsMsp4 = findRegionInMsp("4", data.Value.region);
                HashSet<Models.Region> regionsNoMsp = findRegionInMsp("other", data.Value.region);

                List<Models.Region> persentsRegionsMsp1 = GetPersents(95, regionsMsp1.ToList());
                List<Models.Region> persentsRegionsMsp4 = GetPersents(95, regionsMsp4.ToList());
                List<Models.Region> persentsRegionsNoMsp = GetPersents(95, regionsNoMsp.ToList());

                persentsFullRegionsMsp1.AddRange(persentsRegionsMsp1);
                persentsFullRegionsMsp4.AddRange(persentsRegionsMsp4);
                persentsFullRegionsNoMsp.AddRange(persentsRegionsNoMsp);

                FullRegionsMsp1.AddRange(regionsMsp1);
                FullRegionsMsp4.AddRange(regionsMsp4);
                FullRegionsNoMsp.AddRange(regionsNoMsp);

                dataTable[0].Rows.Add(data.Key, AllCountList[data.Key], data.Value.region.Count,
                    regionsMsp1.Count,
                    regionsMsp4.Count,
                    regionsNoMsp.Count,
                    getSliceFromList(regionsMsp1.ToList()),
                    getSliceFromList(regionsMsp4.ToList()),
                    getSliceFromList(regionsNoMsp.ToList()),
                    persentsRegionsMsp1.Count,
                    persentsRegionsMsp4.Count,
                    persentsRegionsNoMsp.Count
                    );
            }

            dataTable[1] = GetListDataTable(persentsFullRegionsMsp1);
            dataTable[2] = GetListDataTable(persentsFullRegionsMsp4);
            dataTable[3] = GetListDataTable(persentsFullRegionsNoMsp);

            dataTable[4] = GetListDataTable(FullRegionsMsp1);
            dataTable[5] = GetListDataTable(FullRegionsMsp4);
            dataTable[6] = GetListDataTable(FullRegionsNoMsp);

            return dataTable;
        }
    }
}
