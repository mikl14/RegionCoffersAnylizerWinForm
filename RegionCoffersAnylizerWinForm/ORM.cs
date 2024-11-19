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
        public List<Models.Region> regionsList;

        public List<Models.Coffers> coffersList;

        public Dictionary<string, List<Models.Region>> regionsDictionary;

        public Dictionary<string, Models.Coffers> coffersDictionary;

        public void InitDatas(NalogiContext db, string tableName,string coffersTableName)
        {
            

            FormattableString SqlRequest = FormattableStringFactory.Create("SELECT * FROM coffers2024." + coffersTableName);
            coffersList = db.Coffers.FromSql(SqlRequest).ToList();
            coffersDictionary = coffersList.GroupBy(x => x.Inn).ToDictionary(g => g.Key, g => g.FirstOrDefault());


            SqlRequest = FormattableStringFactory.Create("SELECT * FROM gs2024." + tableName);
            regionsList = (List<Models.Region>)db.Regions.FromSql(SqlRequest).ToList();
            regionsDictionary = regionsList.Where(x => x.Inn != null) .GroupBy(x => x.Oktmo ).ToDictionary(g => g.Key, g => g.ToList());

        }

        public DataTable getRegionDataTable()
        {

            DataTable dataTable = new DataTable();

            dataTable.Columns.Add("Inn", typeof(string));
            dataTable.Columns.Add("Name", typeof(string));
            dataTable.Columns.Add("Profession", typeof(string));

            foreach(Models.Region region in regionsList)
            {
                dataTable.Rows.Add(region.Inn, region.Naimobj, region.Okpo);
            }

            return dataTable;
        }

        public List<Models.Region> getPercents(List<Models.Region> regions)
        {

            regions = regions.OrderByDescending(item => item.Viruchka).ToList();
            decimal sumSlice = 0;

            decimal comulativeSlice = 0;

            List<Models.Region> persentRegionsList = new List<Models.Region>();

            foreach (Models.Region region in regions)
            {
                try
                {
                    sumSlice += coffersDictionary[region.Inn].Slice;
                 }
                catch (Exception ex)
                {

                }
            }

            foreach (Models.Region region in regions)
            {
                try
                {
                    comulativeSlice += coffersDictionary[region.Inn].Slice;

                    if(comulativeSlice >= sumSlice* (decimal)0.95)
                    {
                        persentRegionsList.Add(region);
                        return persentRegionsList;
                    }
                    else
                    {
                        persentRegionsList.Add(region);
                    }
                }
                catch (Exception ex)
                {

                }
            }

        }
    }
}
