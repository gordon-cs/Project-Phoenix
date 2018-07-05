//using Phoenix.DapperDal;
//using Phoenix.Services;
//using System.Collections.Generic;
//using System.Configuration;
//using System.Diagnostics;
//using System.Linq;
//using Xunit;
//using Xunit.Abstractions;

//namespace Phoenix.UnitTests
//{
//    public class AdminDashboardServiceTests
//    {
//        private readonly IDal Dal;

//        private readonly AdminDashboardService AdminService;

//        private readonly DashboardService DashboardService;

//        private readonly RciInputService RciInputService;

//        private readonly ITestOutputHelper Output;

//        public AdminDashboardServiceTests(ITestOutputHelper _output)
//        {
//            var connectionString = ConfigurationManager.ConnectionStrings["RCIDatabase"].ConnectionString;

//            this.Dal = new DapperDal.DapperDal(connectionString);

//            this.AdminService = new AdminDashboardService(this.Dal);

//            this.DashboardService = new DashboardService(true);

//            this.RciInputService = new RciInputService();

//            this.Output = _output;

//            SlapperAutoMapperInit.Initialize();
//        }

//        [Fact]
//        public void TestComparison_Poco()
//        {
//            var sessions = new List<string> { "201801", "201709" };

//            var buildings = new List<string> { "TAV" };

//            var keyword = "Tia";

//            double test1FunctionElapsedTime = 0.0;

//            int iterations = 5;

//            for (var i = 0; i < iterations; i++)
//            {
//                var watch = Stopwatch.StartNew();

//                this.Dal.FetchRciBySessionAndBuilding(sessions, buildings);

//                watch.Stop();

//                test1FunctionElapsedTime += watch.ElapsedMilliseconds;
//            }

//            this.Output.WriteLine($"TEST1 Average Elapsed Time: {test1FunctionElapsedTime / (double)iterations}");
//        }
//    }
//}
