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
        public async Task Test_�����X�e�[�^�XOK()
        {
            // HttpMessageHandler �̃��b�N�����B����͕K�{�̃R�[�h�ɂȂ�B
            // handler.CreateClient() �� HttpClient ����邱�Ƃ��ł���B
            // .CreateClientFactory() �� HttpClientFactory ����邱�Ƃ��ł���B
            var handler    = new Mock<HttpMessageHandler>();
            var httpClient = handler.CreateClient();

            // HttpMessageHandler.SetupAnyRequest().ReturnsJsonResponse(model)
            // ����ŉ��炩�̉����iAnyRequest�j�ŕԂ����X�|���X�f�[�^��ݒ肷��B
            // .SetupRequest() ���g�����ƂŁA����̃��\�b�h(GET, POST) �� URL ���w�肷�邱�Ƃ��ł���B
            // .ReturnsJsonResponse() �̑������ŉ������̃w�b�_�[�Ȃǂ��ς�����B
            var model = new Model {id = 123, name = "test user", age = 20};
            handler.SetupAnyRequest().ReturnsJsonResponse(model);
            //handler.SetupAnyRequest().ReturnsJsonResponse(model, configure: response => response.Content.Headers.Add("X-Any-Value", "test"));

            // ����ȍ~�͕��ʂɃe�X�g�R�[�h���������B
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