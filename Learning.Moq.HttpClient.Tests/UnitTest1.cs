namespace Learning.Moq.HttpClient.Tests
{
    using System.Net;
    using System.Text.Json;
    using global::Moq;
    using global::Moq.Contrib.HttpClient;

    public class HttpClientTest
    {
        // cf. https://github.com/maxkagamine/Moq.Contrib.HttpClient

        [Fact]
        public async Task Test_応答ステータスOK()
        {
            // HttpMessageHandler のモックを作る。これは必須のコードになる。
            // handler.CreateClient() で HttpClient を作ることができる。
            // .CreateClientFactory() で HttpClientFactory も作ることができる。
            var handler    = new Mock<HttpMessageHandler>();
            var httpClient = handler.CreateClient();

            // HttpMessageHandler.SetupAnyRequest().ReturnsJsonResponse(model)
            // これで何らかの応答（AnyRequest）で返すレスポンスデータを設定する。
            // .SetupRequest() を使うことで、特定のメソッド(GET, POST) や URL を指定することもできる。
            // .ReturnsJsonResponse() の第二引数で応答時のヘッダーなども変えられる。
            var model = new Model {id = 123, name = "test user", age = 20};
            handler.SetupAnyRequest().ReturnsJsonResponse(model);
            //handler.SetupAnyRequest().ReturnsJsonResponse(model, configure: response => response.Content.Headers.Add("X-Any-Value", "test"));

            // これ以降は普通にテストコード書くだけ。
            var response = await httpClient.GetAsync("https://example.test.learning/httpclient");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var json        = await response.Content.ReadAsStringAsync();
            var actualModel = JsonSerializer.Deserialize<Model>(json);

            Assert.Equal(123, actualModel.id);
            Assert.Equal("test user", actualModel.name);
            Assert.Equal(20, actualModel.age);
        }
    }

    public sealed class Model
    {
        public int id { get; set; }

        public string name { get; set; }

        public int age { get; set; }
    }
}