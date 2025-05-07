using Precursor.Cache.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagTool.BlamFile;
using TagTool.IO;

namespace Precursor.Cache.Resolvers
{
    public class CacheMCCResolver
    {
        public List<string> Halo1Files { get; set; }
        public List<string> Halo2Files { get; set; }
        public List<string> Halo3Files { get; set; }
        public List<string> Halo3ODSTFiles { get; set; }
        public List<string> HaloReachFiles { get; set; }
        public List<string> Halo4Files { get; set; }
        public List<string> Halo2AMPFiles { get; set; }

        public CacheMCCResolver()
        {
            Halo1Files = new List<string>();
            Halo2Files = new List<string>();
            Halo3Files = new List<string>();
            Halo3ODSTFiles = new List<string>();
            HaloReachFiles = new List<string>();
            Halo4Files = new List<string>();
            Halo2AMPFiles = new List<string>();
        }

        public void VerifyBuild(CacheObject.CacheBuildObject build)
        {
            if (string.IsNullOrEmpty(build.Path))
            {
                Console.WriteLine($"> Cache Type: {build.Build} - Null or Empty Path Detected, Skipping Verification...");
                return;
            }
            else if (!Path.Exists(build.Path))
            {
                Console.WriteLine($"> Cache Type: {build.Build} - Unable to Locate Directory, Skipping Verification...");
                return;
            }
            else
            {
                
            }
        }
    }
}
