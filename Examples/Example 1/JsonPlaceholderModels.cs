using System;
using OneCiel.System.Dynamics;

namespace Examples
{
    /// <summary>
    /// Strongly-typed model for JSONPlaceholder User API.
    /// Inherits from DynamicDictionary to provide both dynamic and strongly-typed access.
    /// API: https://jsonplaceholder.typicode.com/users
    /// </summary>
    public class JsonPlaceholderUser : DynamicDictionary
    {
        // Strongly-typed properties with safe type conversion
        public int Id => GetValue<int>("id");
        public string Name => GetValue<string>("name");
        public string Username => GetValue<string>("username");
        public string Email => GetValue<string>("email");
        public string Phone => GetValue<string>("phone");
        public string Website => GetValue<string>("website");

        // Nested objects can be accessed as DynamicDictionary
        public dynamic Address => this["address"];
        public dynamic Company => this["company"];

        // Convenience methods for nested data
        public string GetCity() => Address?.city ?? "Unknown";
        public string GetCompanyName() => Company?.name ?? "Unknown";

        /// <summary>
        /// Returns a formatted string representation of the user.
        /// </summary>
        public override string ToString()
        {
            return $"User #{Id}: {Name} ({Email})";
        }
    }

    /// <summary>
    /// Strongly-typed model for JSONPlaceholder Post API.
    /// API: https://jsonplaceholder.typicode.com/posts
    /// </summary>
    public class JsonPlaceholderPost : DynamicDictionary
    {
        public int Id => GetValue<int>("id");
        public int UserId => GetValue<int>("userId");
        public string Title => GetValue<string>("title");
        public string Body => GetValue<string>("body");

        /// <summary>
        /// Returns a formatted string representation of the post.
        /// </summary>
        public override string ToString()
        {
            return $"Post #{Id} by User {UserId}: {Title}";
        }
    }

    /// <summary>
    /// Strongly-typed model for JSONPlaceholder Comment API.
    /// API: https://jsonplaceholder.typicode.com/comments
    /// </summary>
    public class JsonPlaceholderComment : DynamicDictionary
    {
        public int Id => GetValue<int>("id");
        public int PostId => GetValue<int>("postId");
        public string Name => GetValue<string>("name");
        public string Email => GetValue<string>("email");
        public string Body => GetValue<string>("body");

        /// <summary>
        /// Returns a formatted string representation of the comment.
        /// </summary>
        public override string ToString()
        {
            return $"Comment #{Id} on Post {PostId} by {Email}";
        }
    }

    /// <summary>
    /// Strongly-typed model for JSONPlaceholder Album API.
    /// API: https://jsonplaceholder.typicode.com/albums
    /// </summary>
    public class JsonPlaceholderAlbum : DynamicDictionary
    {
        public int Id => GetValue<int>("id");
        public int UserId => GetValue<int>("userId");
        public string Title => GetValue<string>("title");

        /// <summary>
        /// Returns a formatted string representation of the album.
        /// </summary>
        public override string ToString()
        {
            return $"Album #{Id}: {Title}";
        }
    }

    /// <summary>
    /// Strongly-typed model for JSONPlaceholder Photo API.
    /// API: https://jsonplaceholder.typicode.com/photos
    /// </summary>
    public class JsonPlaceholderPhoto : DynamicDictionary
    {
        public int Id => GetValue<int>("id");
        public int AlbumId => GetValue<int>("albumId");
        public string Title => GetValue<string>("title");
        public string Url => GetValue<string>("url");
        public string ThumbnailUrl => GetValue<string>("thumbnailUrl");

        /// <summary>
        /// Returns a formatted string representation of the photo.
        /// </summary>
        public override string ToString()
        {
            return $"Photo #{Id}: {Title}";
        }
    }

    /// <summary>
    /// Strongly-typed model for JSONPlaceholder Todo API.
    /// API: https://jsonplaceholder.typicode.com/todos
    /// </summary>
    public class JsonPlaceholderTodo : DynamicDictionary
    {
        public int Id => GetValue<int>("id");
        public int UserId => GetValue<int>("userId");
        public string Title => GetValue<string>("title");
        public bool Completed => GetValue<bool>("completed");

        /// <summary>
        /// Returns a formatted string representation of the todo.
        /// </summary>
        public override string ToString()
        {
            var status = Completed ? "✓" : "✗";
            return $"{status} Todo #{Id}: {Title}";
        }
    }
}
