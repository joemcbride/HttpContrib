HttpContrib
===========

A simple http library to work with web apis.  Runs on .NET 4.0 and Silverlight 5.

See the original blog posts:

http://uicraftsman.com/blog/2011/06/08/consuming-wcf-web-apis-in-windows-phone/
http://uicraftsman.com/blog/2011/01/23/consuming-wcf-web-rest-apis-in-silverlight/

#Get all people

    SimpleHttpClient client = new SimpleHttpClient("http://localhost:1182/people");
    // can also use MediaType.Json, which is the default
    client.Accept = MediaType.Xml;
    
    var query = client.CreateQuery<Person>();
    
    HandleQuery(query);

#Get person by ID

    int id;
    if (Int32.TryParse(this.uxPersonID.Text, out id))
    {
        SimpleHttpClient client = new SimpleHttpClient("http://localhost:1182/people");
    
        var query = client.CreateQuery<Person>().Where(c => c.ID, id);
    
        HandleQuery(query);
    }

#Get top 3 people

    SimpleHttpClient client = new SimpleHttpClient("http://localhost:1182/people");
    
    var query = client.CreateQuery<Person>().Take(3);
    
    HandleQuery(query);
    
#Get 3rd person

    SimpleHttpClient client = new SimpleHttpClient("http://localhost:1182/people");
    
    var query = client.CreateQuery<Person>().Skip(2).Take(1);
    
    HandleQuery(query);
    
#Handle query method used above

    private void HandleQuery(HttpQuery<Person> query)
    {
        var task = query.ExecuteAsync();
        task.ContinueWith(t =>
        {
            Execute.OnUIThread(() =>
            {
                if (!t.IsFaulted && t.IsCompleted && t.Result != null)
                {
                    t.Result.Apply(p => { Debug.WriteLine("Person: {0}", p); });
                }
            });
        });
    }
    
#Create new person

    Uri uri = new Uri("http://localhost:1182/people");
    
    SimpleHttpClient client = new SimpleHttpClient(uri.ToString());
    
    var contact = new Person { ID = 5, Name = personName.Text };
    
    var stream = contact.WriteObjectAsXml();
    
    var request = new HttpRequestMessage(HttpMethod.Post);
    request.Accept = MediaType.Xml;
    request.ContentType = MediaType.Xml;
    request.RequestUri = uri;
    request.Content = stream;
    
    var task = client.SendAsync(request);
    task.ContinueWith(t =>
    {
        Execute.OnUIThread(() =>
        {
            var person = t.Result.ReadXmlAsObject<Person>();
    
            if (person != null)
            {
                Debug.WriteLine("Person: {0}", person);
            }
        });
    });
