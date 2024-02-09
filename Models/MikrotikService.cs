//using MikrotikDotNet;

//namespace WebApplication1.Models
//{
//    public class MikrotikService
//    {
//        public List<string> GetQueueTypes(string ipAddress, string username, string password)
//        {
//            List<string> queueTypes = new List<string>();

//            try
//            {
//                using (var conn = new MKConnection(ipAddress, username, password))
//                {
//                    conn.Open();

//                    var cmd = conn.CreateCommand("/queue/simple/print"); // Adjust the path based on your Mikrotik configuration
//                    var response = cmd.ExecuteReaderDynamic();

//                    // Assuming the response contains a property named "name" for each queue type
//                    foreach (var item in response)
//                    {
//                        if (item.TryGetValue("name", out var queueName))
//                        {
//                            queueTypes.Add(queueName.ToString());
//                        }
//                    }
//                }

//                return queueTypes;
//            }
//            catch (Exception ex)
//            {
//                // Handle connection or execution errors
//                Console.WriteLine($"Error: {ex.Message}");
//                return null;
//            }
//        }
//    }
//}
