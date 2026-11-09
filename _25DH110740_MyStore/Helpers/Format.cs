using System.Globalization;
using System.Web;
using System;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;

namespace _25DH110740_MyStore.Helpers
{
    public static class Format
    {
        private static readonly CultureInfo Vi = new CultureInfo("vi-VN");
        public static string Money(decimal v) { return string.Format(Vi, "{0:N0}₫", v); }
        public static string Money(this System.Web.Mvc.HtmlHelper h, decimal v) { return Money(v); }
        public static MvcHtmlString Pager(this System.Web.Mvc.HtmlHelper h, IPagedList list, Func<int, string> url)
        {
            if (list.PageCount <= 1) return MvcHtmlString.Empty;
            return h.PagedListPager(list, url, new PagedListRenderOptions
            {
                ContainerDivClasses = new[] { "text-center" },
                UlElementClasses = new[] { "pagination" },
                DisplayLinkToFirstPage = PagedListDisplayMode.Never,
                DisplayLinkToLastPage = PagedListDisplayMode.Never,
                LinkToPreviousPageFormat = "&laquo;",
                LinkToNextPageFormat = "&raquo;"
            });
        }
        public static string Img(this System.Web.Mvc.UrlHelper u, string path)
        {
            if (string.IsNullOrEmpty(path)) path = "~/Content/images/products/placeholder.svg";
            return u.Content(path);
        }
    }
}
