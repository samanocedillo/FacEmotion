﻿using FacEmotion.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FacEmotion.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePageDetail : ContentPage
    {
        static Stream streamCopy;

        public HomePageDetail()
        {
            InitializeComponent();
        }

        async void btnFoto_Clicked(object sender, EventArgs e)
        {
            var usarCamara = ((Button)sender).Text.Contains("cámara");
            var file = await TakePhoto.TomarFoto(usarCamara);
            panelResultados.Children.Clear();

            imgFoto.Source = ImageSource.FromStream(() =>
            {
                var stream = file.GetStream();
                streamCopy = new MemoryStream();
                stream.CopyTo(streamCopy);
                stream.Seek(0, SeekOrigin.Begin);
                file.Dispose();

                return stream;
            });
        }

        async void btnAnalizarEmociones_Clicked(object sender, EventArgs e)
        {
            if (streamCopy != null)
            {
                streamCopy.Seek(0, SeekOrigin.Begin);
                var emociones = await
                EmotionAPI.ObtenerEmociones(streamCopy);
                if (emociones != null)
                {
                    lblResultado.Text = " ---Análisis de Emociones---";
                    DibujarResultados(emociones);
                }
                else lblResultado.Text = " ---No se detectó una cara---";
            }
            else lblResultado.Text = " ---No has seleccionado una imagen---";
        }

        void DibujarResultados(Dictionary<string, float> emociones)
        {
            panelResultados.Children.Clear();
            foreach (var emocion in emociones)
            {
                Label lblEmocion = new Label()
                {
                    Text = emocion.Key,
                    TextColor = Color.Blue,
                    WidthRequest = 90
                };

                BoxView box = new BoxView()
                {
                    Color = Color.Lime,
                    WidthRequest = 150 * emocion.Value,
                    HeightRequest = 30,
                    HorizontalOptions = LayoutOptions.StartAndExpand
                };

                Label lblPorcentaje = new Label()
                {
                    Text = emocion.Value.ToString("P4"),
                    TextColor = Color.Maroon
                };

                StackLayout panel = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal
                };

                panel.Children.Add(lblEmocion);
                panel.Children.Add(box);
                panel.Children.Add(lblPorcentaje);
                panelResultados.Children.Add(panel);
            }

        }

    }
}