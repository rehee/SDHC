using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace System
{
    public static partial class E
    {
        public static Action<int> DefaultDeleteContentById(Func<int, IContent> getFromId, Action<IContent> deleteContent)
        {
            return id => deleteContent(getFromId(id));

        }

        public static Action<IContent> DefaultDeleteContent(Action<IContent> service, Action after)
        {
            return content =>
            {
                service(content);
                after();
            };
        }
        public static Action<IEnumerable<int>> DefaultDeleteContentsByIds(Action<int> deleteContentByIdFunc)
        {
            return ids =>
            {
                ids.ToList().ForEach(b => deleteContentByIdFunc(b));
            };
        }
        public static Action<IEnumerable<IContent>> DefaultDeleteContents(Action<IContent> deleteContent)
        {
            return contents =>
            {
                contents.ToList().ForEach(b => deleteContent(b));
            };
        }
        public static Action EmptyBin { get; set; } = () => Console.WriteLine("");

        public static Action<int> DeleteContentById { get; set; }
        public static Action<IContent> DeleteContent { get; set; }
        public static Action<IEnumerable<int>> DeleteContentsByIds { get; set; }
        public static Action<IEnumerable<IContent>> DeleteContents { get; set; }

        public static void InitDelete(IContentService service, Func<int, IContent> GetById, Func<IEnumerable<int>, IEnumerable<IContent>> GetByIds, bool emptyAfterDelete = true)
        {
            if (emptyAfterDelete)
                EmptyBin = () => service.EmptyRecycleBin();

            DeleteContent = DefaultDeleteContent(b => service.Delete(b), EmptyBin);
            DeleteContentById = DefaultDeleteContentById(GetById, DeleteContent);
            DeleteContentsByIds = DefaultDeleteContentsByIds(DeleteContentById);
            DeleteContents = DefaultDeleteContents(DeleteContent);
        }
    }
}