using System;
using System.Collections.Generic;
using System.Text;

namespace AppBongBan.Views.News
{
    public interface INewTab
    {
        bool IsLoad();

        void InitData();
    }
}
