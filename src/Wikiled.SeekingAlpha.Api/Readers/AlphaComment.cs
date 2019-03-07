using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Wikiled.SeekingAlpha.Api.Readers
{
    public class AlphaComment
    {
        public int Id { get; set; }

        public string Content { get; set; }

        [JsonProperty(PropertyName = "user_id")]
        public int UserId { get; set; }

        [JsonProperty(PropertyName = "user_id_code")]
        public string UserIdCode { get; set; }

        [JsonProperty(PropertyName = "created_on")]
        public DateTime CreatedOn { get; set; }

        [JsonProperty(PropertyName = "parent_id")]
        public int? ParentId { get; set; }

        [JsonProperty(PropertyName = "discussion_id")]
        public int DiscussionId { get; set; }

        [JsonProperty(PropertyName = "user_nick")]
        public string UserNick { get; set; }

        [JsonProperty(PropertyName = "user_mywebsite_url")]
        public object UserMywebsiteUrl { get; set; }

        [JsonProperty(PropertyName = "commenter_url")]
        public string CommenterUrl { get; set; }

        [JsonProperty(PropertyName = "author_slug")]
        public string AuthorSlug { get; set; }

        [JsonProperty(PropertyName = "profile_add_to_url")]
        public string ProfileAddToUrl { get; set; }

        public int Level { get; set; }

        [JsonProperty(PropertyName = "belongs_to_sa_editor")]
        public bool BelongsToSaEditor { get; set; }

        [JsonProperty(PropertyName = "html_anchor")]
        public string HtmlAnchor { get; set; }

        public string Uri { get; set; }

        [JsonProperty(PropertyName = "authors_comment_flag")]
        public bool AuthorsCommentFlag { get; set; }

        [JsonProperty(PropertyName = "is_premium_author")]
        public bool IsPremiumAuthor { get; set; }

        [JsonProperty(PropertyName = "is_deactivated_author")]
        public bool IsDeactivatedAuthor { get; set; }

        public int Likes { get; set; }

        public Dictionary<string, AlphaComment> Children { get; set; }
    }
}
