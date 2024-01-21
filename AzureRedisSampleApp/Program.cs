using StackExchange.Redis;

string connectionString = "AzRedisCacheforsaitest.redis.cache.windows.net:6380,password=2SjDI6s6Im5uci1C5yeljeFBqc30OFdWmAzCaOhMEFQ=,ssl=True,abortConnect=False";

using (var cache = ConnectionMultiplexer.Connect(connectionString))
{
    IDatabase db = cache.GetDatabase();

    // Snippet below executes a PING to test the server connection
    var result = await db.ExecuteAsync("ping");
    Console.WriteLine($"PING = {result.Type} : {result}");

    // Call StringSetAsync on the IDatabase object to set the key "test:key" to the value "100"
    bool setValue = await db.StringSetAsync("test:key", "100");
    Console.WriteLine($"SET: {setValue}");

    // StringGetAsync retrieves the value for the "test" key
    string getValue = await db.StringGetAsync("test:key");
    Console.WriteLine($"GET: {getValue}");
}
