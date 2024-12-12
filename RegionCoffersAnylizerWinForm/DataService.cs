using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RegionCoffersAnylizerWinForm
{

    public struct Record
    {
        public Models.Region region;
        //public List<Models.Region> region;
        public Models.Coffers coffer;

        public Record(Models.Region region, Models.Coffers coffer) { this.region = region; this.coffer = coffer; }

        public override bool Equals(object obj)
        {
            if (obj is Record other)
            {
                return this.region.Inn == other.region.Inn;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return region.Inn.GetHashCode();
        }


    }

    internal class DataService
    {
        public Dictionary<string, decimal> AllCountList = new Dictionary<string, decimal>();
        public Dictionary<string, Models.Coffers> coffersDictionary = new Dictionary<string, Models.Coffers>();
        public List<Models.Region> regionsList = new List<Models.Region>();
        public Dictionary<string, List<Models.Region>> OktmoRegionsDictionary;

        public DataService(List<Models.Region> regionsList, Dictionary<string, Models.Coffers> coffersDictionary, Dictionary<string,decimal> countAll)
        {
            this.regionsList = regionsList;
            this.coffersDictionary = coffersDictionary;

            OktmoRegionsDictionary = regionsList
               .GroupBy(x => x.Oktmof.Substring(0, 8)).ToDictionary(g => g.Key, g => g.ToList());

            AllCountList = countAll;

   
        }

        public static int getCountDistinct(List<Models.Region> reg)
        {
            HashSet<Models.Region> modelsReg = new HashSet<Models.Region>();

            foreach (var region in reg)
            {
                modelsReg.Add(region);
            }

            return modelsReg.Count;
        }

        public decimal getSliceFromList(List<Models.Region> regionList)
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

            if (regions.Count <= 4)
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

        public Dictionary<string, HashSet<Record>> getRecordsByOktmo()
        {
            Dictionary<string, HashSet<Record>> oktmoGroupedMap = new Dictionary<string, HashSet<Record>>();

            foreach (var dataEntry in OktmoRegionsDictionary)
            {
                HashSet<Record> records = new HashSet<Record>();
                foreach (var region in dataEntry.Value)
                {

                    records.Add(new Record(region, coffersDictionary[region.Inn]));
                    
                }
                oktmoGroupedMap.Add(dataEntry.Key, records);
            }

            return oktmoGroupedMap;
        }

        public HashSet<Models.Region> getHashSetByPrimaryOktmo(string oktmoPrimary)
        {
            return OktmoRegionsDictionary[oktmoPrimary].ToHashSet();
        }

    }
}
