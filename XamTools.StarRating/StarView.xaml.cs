using FFImageLoading.Svg.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace XamTools.StarRating
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StarView : ContentView
    {
        public event EventHandler<object> ItemSelectedEvent;
        public object Item
        {
            get { return starBehavior.CustomerItem; }
            set { starBehavior.CustomerItem = value; }
        }

        //  public StarBehavior starBehavior { get; set; }

        public StarView(string GroupName)
        {
            InitializeComponent();
            this.starBehavior.GroupName = GroupName;
            //  this.starBehavior = _starBehavior;
            // imgStarred.SetBinding(SvgCachedImage.IsVisibleProperty, "starBehavior.IsStarred");
            // this.starBehavior.ItemBehaviorSelectedEvent += starBehavior_ItemBehaviorSelectedEvent;
            //  this.starBehavior.IsStarredChangedEvent += StarBehavior_IsStarredChangedEvent;
            //  grdImages.Behaviors.Add(this.starBehavior);
            //  imgStarred.IsVisible = false;
        }

        //private void StarBehavior_IsStarredChangedEvent(object sender, bool e)
        //{
        //    imgStarred.IsVisible = e;
        //}

        private void starBehavior_ItemBehaviorSelectedEvent(object sender, EventArgs e)
        {
            EventHandler<object> handler = ItemSelectedEvent;

            if (handler != null)
            {
                handler(this, null);
            }
        }
    }
}