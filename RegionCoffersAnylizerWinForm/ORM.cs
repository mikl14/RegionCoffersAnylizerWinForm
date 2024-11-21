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

            public List<Models.Region> region;
            public Models.Coffers coffer;

            public Record(List<Models.Region> region, Models.Coffers coffer) { this.region = region; this.coffer = coffer; }

            public void setReg(int reg) { this.countReg = reg; }
        }

        public List<Models.Region> regionsList;

        public Dictionary<string, int> AllCountList = new Dictionary<string, int>();

        public List<Models.Coffers> coffersList;

        public Dictionary<string,Record> dataDictionary = new Dictionary<string, Record>();
        Dictionary<string, List<Models.Region>> regionsDictionary;

        Dictionary<string, Models.Coffers> coffersDictionary;


        public void InitDatas(NalogiContext db, string tableName,string coffersTableName)
        {
     

            List<Models.Region> distinctRegionsList;

         Dictionary<string, List<Models.Region>> OktmoRegionsDictionary;

        

         FormattableString SqlRequest = FormattableStringFactory.Create("SELECT * FROM coffers2024." + coffersTableName);
            coffersList = db.Coffers.FromSql(SqlRequest).ToList();
            coffersDictionary = coffersList.GroupBy(x => x.Inn).ToDictionary(g => g.Key, g => g.FirstOrDefault());


            SqlRequest = FormattableStringFactory.Create("SELECT * FROM gs2024." + tableName);
            regionsList = (List<Models.Region>)db.Regions.FromSql(SqlRequest).ToList();
            regionsDictionary = regionsList.Where(x => x.Inn != null) .GroupBy(x => x.Inn).ToDictionary(g => g.Key, g => g.ToList());

          

            OktmoRegionsDictionary = regionsList.Where(x => x.Inn != null)
                .GroupBy(x => x.Oktmo.Substring(0,8)).ToDictionary(g => g.Key, g => g.ToList());


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
            HashSet<string> inns = new HashSet<string>();

            foreach (var region in reg)
            {
                inns.Add(region.Inn);
            }

            return inns.Count;

        }
        public Dictionary<string, Record> regroupByOktmo(Dictionary<string, Record> dataDictionary)
        {
            Dictionary<string,Record> oktmoGroupedMap = new Dictionary<string,Record>();

            var noDataRegionsListDistict = regionsList.GroupBy(p => p.Inn).Select(g => g.First()).ToList();

            foreach (var dataEntry in dataDictionary)
            {
                for (int i = 0; i < dataEntry.Value.region.Count; i++)
                {
                    string primaryOKTMO = dataEntry.Value.region[i].Oktmo.Substring(0, 8);

                    if (!oktmoGroupedMap.ContainsKey(primaryOKTMO))
                    {
                        Record buf = new Record();

               

                        buf.coffer = dataEntry.Value.coffer;
                        buf.region = new List<Models.Region>();
                        buf.region.Add(dataEntry.Value.region[i]);

                        oktmoGroupedMap.Add(primaryOKTMO, buf);
                    }
                    else
                    {
                      
                            oktmoGroupedMap[primaryOKTMO].region.Add(dataEntry.Value.region[i]);
                          
             
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


            foreach (var region in regions)
            {
                coffersBuf.Add(coffersDictionary[region.Inn]);
            }

            coffersBuf.Sort((x, y) => y.Slice.CompareTo(x.Slice));

            List<Models.Region> result = new List<Models.Region> ();

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

        public DataTable getRegionDataTable()
        {

            Dictionary<string, Record> oktmoGroupedMap = regroupByOktmo(dataDictionary);

           
            Dictionary<string, Models.Region> MspPayers4 = new Dictionary<string, Models.Region>();
            Dictionary<string, Models.Region> NoMspPayers = new Dictionary<string, Models.Region>();


            DataTable dataTable = new DataTable();

            dataTable.Columns.Add("OKTMO", typeof(string));
            dataTable.Columns.Add("CountAll", typeof(int));
            dataTable.Columns.Add("CountPayers", typeof(int));
            dataTable.Columns.Add("PaersMsp1", typeof(int));
            dataTable.Columns.Add("PaersMsp4", typeof(int));
            dataTable.Columns.Add("PaersNoMsp", typeof(int));
            dataTable.Columns.Add("SliceMsp1", typeof(decimal));
            dataTable.Columns.Add("SliceMsp4", typeof(decimal)) ;
            dataTable.Columns.Add("SliceNoMsp", typeof(decimal));
            dataTable.Columns.Add("PercentsMsp1", typeof(decimal));
            dataTable.Columns.Add("PercentsMsp4", typeof(decimal));
            dataTable.Columns.Add("PercentsNoMsp", typeof(decimal));
            dataTable.Columns.Add("Slice", typeof(string));


  


            HashSet<Models.Region> findRegionInMsp(string param, List< Models.Region> regions)
            {

                
                HashSet <Models.Region> MspPayersResult = new HashSet<Models.Region>();


                    for (int i = 0; i < regions.Count; i++)
                    {
                        if (regions[i].TypeMsp == param || (param == "other" && regions[i].TypeMsp != "1" && regions[i].TypeMsp != "4"))
                        {
        
                                MspPayersResult.Add(regions[i]);
                     
                        }

                    }


      
                return MspPayersResult;
            }

            foreach (var data in oktmoGroupedMap)
            {
                HashSet<Models.Region> regionsMsp1 = findRegionInMsp("1", data.Value.region);
                HashSet<Models.Region> regionsMsp4 = findRegionInMsp("4", data.Value.region);
                HashSet<Models.Region> regionsNoMsp = findRegionInMsp("other", data.Value.region);

                dataTable.Rows.Add(data.Key, AllCountList[data.Key], data.Value.region.Count,
                    regionsMsp1.Count,
                    regionsMsp4.Count,
                    regionsNoMsp.Count,
                    getSliceFromList(regionsMsp1.ToList()),
                    getSliceFromList(regionsMsp4.ToList()),
                    getSliceFromList(regionsNoMsp.ToList()),
                    GetPersents(95, regionsMsp1.ToList()).Count,
                    GetPersents(95, regionsMsp4.ToList()).Count,
                    GetPersents(95, regionsNoMsp.ToList()).Count
                    );
            }

            return dataTable;
        }

       
    }
}
