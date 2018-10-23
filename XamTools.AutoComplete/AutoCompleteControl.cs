using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using System.Collections;
using System.Threading;


namespace XamTools.AutoComplete
{
  public  class AutoCompleteControl : ContentView
    {

        ListView listViewSuggestions;
        private ObservableCollection<object> currentSuggestions;
        private Entry entrySearchBox;
        private StackLayout stkBase;

        public AutoCompleteControl()
        {
           
            listViewSuggestions = new ListView();
            listViewSuggestions.ItemTemplate = GetDataTemplate();
            currentSuggestions = new ObservableCollection<object>();
            stkBase = new StackLayout();
            entrySearchBox = new Entry();
            listViewSuggestions.HasUnevenRows = true;
            listViewSuggestions.VerticalOptions = LayoutOptions.FillAndExpand;

            if (string.IsNullOrEmpty(this.DisplayMember))
            {
                this.DisplayMember = ".";
            }

            stkBase.Children.Add(entrySearchBox);

            if (IsListOnTop)
            {
                stkBase.Children.Insert(0, listViewSuggestions);
            }
            else
            {
                stkBase.Children.Add(listViewSuggestions);
            }

            this.Content = stkBase;
            entrySearchBox.TextChanged += entrySearchBox_TextChanged;
            listViewSuggestions.ItemSelected += ListViewSuggestions_ItemSelected;

            SetListViewVisible(false);
            listViewSuggestions.ItemsSource = currentSuggestions;
            listViewSuggestions.BackgroundColor = Color.White;
        }

        private void ListViewSuggestions_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                entrySearchBox.Text = e.SelectedItem.ToString();
                currentSuggestions.Clear();
                SetListViewVisible(false);
                OnSelectedItemChanged(e.SelectedItem);
                suggestionsCts.Cancel();
            }
        }

        private DataTemplate GetDataTemplate()
        {
           
            var personDataTemplate = new DataTemplate(() =>
            {
                var nameLabel = new Label();
                nameLabel.SetBinding(Label.TextProperty, this.DisplayMember);
                nameLabel.TextColor = this.TextColor;
                nameLabel.FontFamily = this.FontFamily;

                return new ViewCell { View = nameLabel };
            });
                     
            return personDataTemplate;
        }


        public static readonly BindableProperty SearchBoxPlaceholderProperty = BindableProperty.Create(nameof(SearchBoxPlaceholder), typeof(string), typeof(AutoCompleteControl), "", BindingMode.TwoWay, null, PlaceHolderChanged);

        public static new readonly BindableProperty BackgroundColorProperty = BindableProperty.Create(nameof(BackgroundColor), typeof(Color), typeof(AutoCompleteControl), Color.White, BindingMode.TwoWay, null, BackgroundColorChanged);

        public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(AutoCompleteControl), null, BindingMode.TwoWay, null, UpdateSelectedItem);

        public static readonly BindableProperty SuggestionListBackgroundColorProperty = BindableProperty.Create(nameof(SuggestionListBackgroundColor), typeof(Color), typeof(AutoCompleteControl), Color.White, BindingMode.TwoWay, null, SuggestionListBackgroundColorChanged);

        public static readonly BindableProperty SuggestionListItemDataTemplateProperty = BindableProperty.Create(nameof(SuggestionListItemDataTemplate), typeof(DataTemplate), typeof(AutoCompleteControl), null, BindingMode.TwoWay, null, SuggestionListItemDataTemplateChanged);

        public static readonly BindableProperty SuggestionsHeightRequestProperty = BindableProperty.Create(nameof(SuggestionListsHeightRequest), typeof(double), typeof(AutoCompleteControl), 250.0, BindingMode.TwoWay, null, SuggestionHeightRequestChanged);

        public static readonly BindableProperty ListSourceProperty = BindableProperty.Create(nameof(ListSource), typeof(IEnumerable<object>), typeof(AutoCompleteControl), null, BindingMode.TwoWay);

        public static readonly BindableProperty SearchBoxBackgroundColorProperty = BindableProperty.Create(nameof(SearchBoxBackgroundColor), typeof(Color), typeof(AutoCompleteControl), Color.White, BindingMode.TwoWay, null, SearchBoxBackgroundColorChanged);

        public static readonly BindableProperty SearchBoxTextColorProperty = BindableProperty.Create(nameof(SearchBoxTextColor), typeof(Color), typeof(AutoCompleteControl), Color.Black, BindingMode.TwoWay, null, SearchBoxTextColorChanged);

        public static readonly BindableProperty PlaceHolderColorProperty = BindableProperty.Create(nameof(PlaceHolderColor), typeof(Color), typeof(AutoCompleteControl), Color.Black, BindingMode.TwoWay, null, PlaceHolderColorPropertyChanged);

        public static readonly BindableProperty SearchBoxHorizontalOptionsProperty = BindableProperty.Create(nameof(SearchBoxHorizontalOption), typeof(LayoutOptions), typeof(AutoCompleteControl), LayoutOptions.FillAndExpand, BindingMode.TwoWay, null, SearchBoxHorizontalOptionsChanged);

        public static readonly BindableProperty TextValueProperty = BindableProperty.Create(nameof(TextValue), typeof(String), typeof(AutoCompleteControl), string.Empty, BindingMode.TwoWay, null, TextValueChanged);

         public static readonly BindableProperty IsListOnTopProperty = BindableProperty.Create(nameof(IsListOnTop), typeof(Boolean), typeof(AutoCompleteControl),false, BindingMode.TwoWay);

        public static readonly BindableProperty SearchBoxVerticalOptionProperty = BindableProperty.Create(nameof(SearchBoxVerticalOption), typeof(LayoutOptions),
                                                                                    typeof(AutoCompleteControl), LayoutOptions.Start,
                                                                                    BindingMode.TwoWay, null, SearchBoxVerticalOptionChanged);


        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(AutoCompleteControl), Color.Black, BindingMode.TwoWay, null, TextLabelValuesChanged);

        public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create(nameof(FontFamily), typeof(string), typeof(AutoCompleteControl), "Century Gothic", BindingMode.TwoWay, null, TextLabelValuesChanged);

        private static void TextLabelValuesChanged(BindableObject obj, object oldValue, object newValue)
        {

            if (obj is AutoCompleteControl AutoCompleteControl)
            {
                AutoCompleteControl.listViewSuggestions.ItemTemplate = AutoCompleteControl.GetDataTemplate();
            }
        }


        public string FontFamily
        {
            get { return (string)GetValue(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }

        public Color TextColor
        {
            get { return (Color)GetValue(TextColorProperty); }
            set { SetValue(TextColorProperty, value); }
        }

        /// <summary>
        /// Occurs when [selected item changed].
        /// </summary>
        public event EventHandler<SelectedItemChangedEventArgs> SelectedItemChanged;

        /// <summary>
        /// Occurs when [text changed].
        /// </summary>
        public event EventHandler<TextChangedEventArgs> TextChanged;

        /// <summary>
        /// Gets the available Suggestions.
        /// </summary>
        /// <value>The available Suggestions.</value>
        public IEnumerable AvailableSuggestions
        {
            get { return currentSuggestions; }
        }


        /// <summary>
        /// Gets or sets the placeholder.
        /// </summary>
        /// <value>The placeholder.</value>
        public string SearchBoxPlaceholder
        {
            get { return (string)GetValue(SearchBoxPlaceholderProperty); }
            set { SetValue(SearchBoxPlaceholderProperty, value); }
        }

        /// <summary>
        /// Gets or sets the color of the search background.
        /// </summary>
        /// <value>The color of the search background.</value>
        public new Color BackgroundColor
        {
            get { return (Color)GetValue(BackgroundColorProperty); }
            set { SetValue(BackgroundColorProperty, value); }
        }


        public string DisplayMember { get; set; }

        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        /// <value>The selected item.</value>
        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }


        /// <summary>
        /// Gets or sets the color of the sugestion background.
        /// </summary>
        /// <value>The color of the sugestion background.</value>
        public Color SuggestionListBackgroundColor
        {
            get { return (Color)GetValue(SuggestionListBackgroundColorProperty); }
            set { SetValue(SuggestionListBackgroundColorProperty, value); }
        }

        /// <summary>
        /// Gets or sets the suggestion item data template.
        /// </summary>
        /// <value>The sugestion item data template.</value>
        public DataTemplate SuggestionListItemDataTemplate
        {
            get { return (DataTemplate)GetValue(SuggestionListItemDataTemplateProperty); }
            set { SetValue(SuggestionListItemDataTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Suggestions.
        /// </summary>
        /// <value>The Suggestions.</value>
        public IEnumerable<object> ListSource
        {
            get { return (IEnumerable<object>)GetValue(ListSourceProperty); }
            set { SetValue(ListSourceProperty, value); }
        }

        /// <summary>
        /// Gets or sets the height of the suggestion.
        /// </summary>
        /// <value>The height of the suggestion.</value>
        public double SuggestionListsHeightRequest
        {
            get { return (double)GetValue(SuggestionsHeightRequestProperty); }
            set { SetValue(SuggestionsHeightRequestProperty, value); }
        }
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string TextValue
        {
            get { return (string)GetValue(TextValueProperty); }
            set { SetValue(TextValueProperty, value); }
        }

        /// <summary>
        /// Gets or sets the color of the text background.
        /// </summary>
        /// <value>The color of the text background.</value>
        public Color SearchBoxBackgroundColor
        {
            get { return (Color)GetValue(SearchBoxBackgroundColorProperty); }
            set { SetValue(SearchBoxBackgroundColorProperty, value); }
        }

        public Color PlaceHolderColor
        {
            get { return (Color)GetValue(PlaceHolderColorProperty); }
            set { SetValue(PlaceHolderColorProperty, value); }
        }

        /// <summary>
        /// Gets or sets the color of the text.
        /// </summary>
        /// <value>The color of the text.</value>
        public Color SearchBoxTextColor
        {
            get { return (Color)GetValue(SearchBoxTextColorProperty); }
            set { SetValue(SearchBoxTextColorProperty, value); }
        }


        /// <summary>
        /// Gets or sets the text horizontal options.
        /// </summary>
        /// <value>The text horizontal options.</value>
        public Boolean IsListOnTop
        {
            get
            {
                return (Boolean)GetValue(IsListOnTopProperty);
            }
            set
            {
                SetValue(IsListOnTopProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the text horizontal options.
        /// </summary>
        /// <value>The text horizontal options.</value>
        public LayoutOptions SearchBoxHorizontalOption
        {
            get { return (LayoutOptions)GetValue(SearchBoxHorizontalOptionsProperty); }
            set { SetValue(SearchBoxHorizontalOptionsProperty, value); }
        }

        /// <summary>
        /// Gets or sets the text vertical options.
        /// </summary>
        /// <value>The text vertical options.</value>
        public LayoutOptions SearchBoxVerticalOption
        {
            get { return (LayoutOptions)GetValue(SearchBoxVerticalOptionProperty); }
            set { SetValue(SearchBoxVerticalOptionProperty, value); }
        }

        /// <summary>
        /// Places the holder changed.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="oldPlaceHolderValue">The old place holder value.</param>
        /// <param name="newPlaceHolderValue">The new place holder value.</param>
        private static void PlaceHolderChanged(BindableObject obj, object oldPlaceHolderValue, object newPlaceHolderValue)
        {
            if (obj is AutoCompleteControl AutoCompleteControl && newPlaceHolderValue is string newPlaceH)
            {
                AutoCompleteControl.entrySearchBox.Placeholder = newPlaceH;
            }
        }


        private static void BackgroundColorChanged(BindableObject obj, object oldValue, object newValue)
        {
            if (obj is AutoCompleteControl AutoCompleteControl && newValue is Color newColor)
            {
                AutoCompleteControl.stkBase.BackgroundColor = newColor;
            }
        }


        /// <summary>
        /// Searches the background color changed.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private static void TextDescriptionPropertyChanged(BindableObject obj, string oldValue, string newValue)
        {
            if (obj is AutoCompleteControl AutoCompleteControl)
            {
                AutoCompleteControl.listViewSuggestions.ItemTemplate = AutoCompleteControl.GetDataTemplate();
            }
        }



        private static void PlaceHolderColorPropertyChanged(BindableObject obj, object oldValue, object newValue)
        {

            if (obj is AutoCompleteControl AutoCompleteControl && AutoCompleteControl.entrySearchBox != null && newValue is Color newColor)
            {
                AutoCompleteControl.entrySearchBox.PlaceholderColor = newColor;
            }
        }


        private static void SuggestionListBackgroundColorChanged(BindableObject obj, object oldValue, object newValue)
        {

            if (obj is AutoCompleteControl AutoCompleteControl && newValue is Color newColor)
            {
                AutoCompleteControl.listViewSuggestions.BackgroundColor = newColor;
            }
        }


        private static void SuggestionHeightRequestChanged(BindableObject obj, object oldValue, object newValue)
        {

            if (obj is AutoCompleteControl AutoCompleteControl && newValue is double newHeight)
            {
                AutoCompleteControl.listViewSuggestions.HeightRequest = newHeight;
            }
        }

        private static void SuggestionListItemDataTemplateChanged(BindableObject obj, object oldValue, object newValue)
        {

            if (obj is AutoCompleteControl AutoCompleteControl && newValue is DataTemplate newtemplate)
            {
                AutoCompleteControl.listViewSuggestions.ItemTemplate = newtemplate;
            }
        }


        private static void SearchBoxVerticalOptionChanged(BindableObject obj, object oldValue, object newValue)
        {

            if (obj is AutoCompleteControl AutoCompleteControl && newValue is LayoutOptions newLayout)
            {
                AutoCompleteControl.entrySearchBox.VerticalOptions = newLayout;
            }
        }


        private static void SearchBoxBackgroundColorChanged(BindableObject obj, object oldValue, object newValue)
        {
            if (obj is AutoCompleteControl AutoCompleteControl && AutoCompleteControl.entrySearchBox != null && newValue is Color newColor)
            {
                AutoCompleteControl.entrySearchBox.BackgroundColor = newColor;
            }
        }

        /// <summary>
        /// Texts the color changed.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private static void SearchBoxTextColorChanged(BindableObject obj, object oldValue, object newValue)
        {

            if (obj is AutoCompleteControl AutoCompleteControl && newValue is Color newColor)
            {
                AutoCompleteControl.entrySearchBox.TextColor = newColor;
            }
        }

        /// <summary>
        /// Texts the horizontal options changed.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private static void SearchBoxHorizontalOptionsChanged(BindableObject obj, object oldValue, object newValue)
        {
            if (obj is AutoCompleteControl AutoCompleteControl && newValue is LayoutOptions layoutOptions)
            {
                AutoCompleteControl.entrySearchBox.VerticalOptions = layoutOptions;
            }
        }

        private static CancellationTokenSource suggestionsCts;
        /// <summary>
        /// Texts the changed.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="oldTextValue">The old place holder value.</param>
        /// <param name="newTextValue">The new place holder value.</param>
        private static void TextValueChanged(BindableObject obj, object oldValue, object newValue)
        {
            suggestionsCts?.Cancel();

            if (obj is AutoCompleteControl control && newValue is string newTextValue)
            {
                var cleanedNewPlaceHolderValue = newTextValue.ToLowerInvariant().Trim();

                if (!string.IsNullOrEmpty(cleanedNewPlaceHolderValue) && control.ListSource != null)
                {
                    suggestionsCts = new CancellationTokenSource();
                    Task.Run(() =>
                    {
                        var filteredSuggestions = new List<object>();
                        if (!string.IsNullOrEmpty(control.DisplayMember))
                        {
                            try
                            {
                                var objList = control.ListSource.ToList<object>();
                                var lastNames = objList.Where(x => x.GetType().GetProperty(control.DisplayMember).GetValue(x).ToString().ToLower().Contains(cleanedNewPlaceHolderValue)).ToList();
                                filteredSuggestions.AddRange(lastNames);
                            }
                            catch (Exception)
                            {
                            }                            
                        }

                        if (filteredSuggestions.Count > 250)
                        {
                            filteredSuggestions = filteredSuggestions.GetRange(0, 250);
                        }

                        Device.BeginInvokeOnMainThread(() =>
                        {
                            control.currentSuggestions.Clear();
                            if (filteredSuggestions.Count > 0)
                            {
                                foreach (var suggestion in filteredSuggestions)
                                {
                                    if (suggestionsCts.Token.IsCancellationRequested) return;
                                    control.currentSuggestions.Add(suggestion);
                                }
                                if (suggestionsCts.Token.IsCancellationRequested) return;
                                control.SetListViewVisible(true);
                            }
                            else
                            {
                                control.SetListViewVisible(false);
                            }
                        });


                    }, suggestionsCts.Token);
                }
                else
                {
                    if (control.currentSuggestions.Count > 0)
                    {
                        control.currentSuggestions.Clear();
                        control.SetListViewVisible(false);
                    }
                    control.SelectedItem = null;
                }
            }
        }


        private void _entText_Unfocused(object sender, FocusEventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                SetListViewVisible(false);
            });
        }

        private void OnSelectedItemChanged(object selectedItem)
        {
            this.SelectedItem = selectedItem;
            this.SelectedItemChanged?.Invoke(this, new SelectedItemChangedEventArgs(selectedItem));
        }

        private static void UpdateSelectedItem(BindableObject bindable, object oldvalue, object newvalue)
        {
            var control = bindable as AutoCompleteControl;

            if (newvalue != null)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    control.entrySearchBox.TextChanged -= control.entrySearchBox_TextChanged;
                    if (newvalue != null)
                    {
                        control.entrySearchBox.Text = newvalue.ToString();
                        control.SetListViewVisible(false);
                    }
                    else
                    {
                        control.entrySearchBox.Text = "";
                    }
                    control.entrySearchBox.TextChanged += control.entrySearchBox_TextChanged;
                });
            }
            else
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    control.entrySearchBox.Text = "";
                    control.SetListViewVisible(false);
                });
            }

        }

        private void entrySearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextValue = e.NewTextValue;
            TextChanged?.Invoke(this, e);
        }


        private void SetListViewVisible(bool show)
        {
            listViewSuggestions.IsVisible = show;
        }
    }
}