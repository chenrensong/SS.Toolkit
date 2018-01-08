namespace SS.Toolkit.Http
{
    public sealed class AsyncCookie
    {
        public AsyncCookie()
        {

        }

        public AsyncCookie(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }

        public AsyncCookie(string cookie)
        {
            var keys = cookie.Split(';');
            if (keys.Length > 0)
            {
                this.Name = keys[0].Split('=')[0];
                this.Value = keys[0].Split('=')[1];
                if (keys.Length > 1)
                {
                    this.Path = keys[1].Split('=')[1];
                }
                try
                {
                    if (keys.Length > 2)
                    {
                        this.Domain = keys[2].Split('=')[1];
                    }
                    if (keys.Length > 3)
                    {
                        this.MaxAge = keys[3].Split('=')[1];
                    }
                }
                catch
                {

                }
            }
        }

        public string Name { get; set; }

        public string Value { get; set; }

        public string Path { get; set; }

        public string MaxAge { get; set; }

        public string Domain { get; set; }

        public override string ToString()
        {
            //if (!string.IsNullOrEmpty(Path))
            //{
            //    builder.Add($"Path={Path}");
            //}
            //if (!string.IsNullOrEmpty(Domain))
            //{
            //    builder.Add($"Domain={Domain}");
            //}
            //if (!string.IsNullOrEmpty(MaxAge))
            //{
            //    builder.Add($"Max-Age={MaxAge}");
            //}
            return string.Join(";", $"{Name}={Value}");
        }

    }
}
