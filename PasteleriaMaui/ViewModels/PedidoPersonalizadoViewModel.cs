using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using PasteleriaMaui.Models;
using PasteleriaMaui.Services;

namespace PasteleriaMaui.ViewModels
{
    public class PedidoPersonalizadoViewModel : INotifyPropertyChanged
    {
        private readonly PastelApiService _pastelService;
        private readonly TransaccionesService _transaccionesService;
        private bool _isSyncing = false;

        public ObservableCollection<CatBizcocho> Bizcochos { get; } = new();
        public ObservableCollection<CatRelleno> Rellenos { get; } = new();
        public ObservableCollection<CatGlaseado> Glaseados { get; } = new();
        public ObservableCollection<CatCategoria> Categorias { get; } = new();
        public ObservableCollection<PastelVisualItem> SaboresCarrusel { get; } = new();

        private string _nombre = "Pastel de Chocolate con Ganache";
        public string Nombre
        {
            get => _nombre;
            set
            {
                if (_nombre != value)
                {
                    _nombre = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _tamanio = "Mediano";
        public string Tamanio
        {
            get => _tamanio;
            set
            {
                if (_tamanio != value)
                {
                    _tamanio = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _fotoUrl = "https://images.unsplash.com/photo-1578985545062-69928b1d9587";
        public string FotoUrl
        {
            get => _fotoUrl;
            set
            {
                if (_fotoUrl != value)
                {
                    _fotoUrl = value;
                    OnPropertyChanged();
                }
            }
        }

        private DateTime _fechaEntrega = DateTime.Today.AddDays(2);
        public DateTime FechaEntrega
        {
            get => _fechaEntrega;
            set
            {
                if (_fechaEntrega != value)
                {
                    _fechaEntrega = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime FechaMinima => DateTime.Today.AddDays(2);

        private string _cantidad = "1";
        public string Cantidad
        {
            get => _cantidad;
            set
            {
                if (_cantidad != value)
                {
                    _cantidad = value;
                    OnPropertyChanged();
                }
            }
        }

        private CatBizcocho? _bizcochoSeleccionado;
        public CatBizcocho? BizcochoSeleccionado
        {
            get => _bizcochoSeleccionado;
            set
            {
                if (_bizcochoSeleccionado != value)
                {
                    _bizcochoSeleccionado = value;
                    OnPropertyChanged();
                    OnBizcochoOGlaseadoCambiado();
                }
            }
        }

        private CatRelleno? _rellenoSeleccionado;
        public CatRelleno? RellenoSeleccionado
        {
            get => _rellenoSeleccionado;
            set
            {
                if (_rellenoSeleccionado != value)
                {
                    _rellenoSeleccionado = value;
                    OnPropertyChanged();
                }
            }
        }

        private CatGlaseado? _glaseadoSeleccionado;
        public CatGlaseado? GlaseadoSeleccionado
        {
            get => _glaseadoSeleccionado;
            set
            {
                if (_glaseadoSeleccionado != value)
                {
                    _glaseadoSeleccionado = value;
                    OnPropertyChanged();
                    OnBizcochoOGlaseadoCambiado();
                }
            }
        }

        private CatCategoria? _categoriaSeleccionada;
        public CatCategoria? CategoriaSeleccionada
        {
            get => _categoriaSeleccionada;
            set
            {
                if (_categoriaSeleccionada != value)
                {
                    _categoriaSeleccionada = value;
                    OnPropertyChanged();
                }
            }
        }

        private PastelVisualItem? _itemCarruselActual;
        public PastelVisualItem? ItemCarruselActual
        {
            get => _itemCarruselActual;
            set
            {
                if (_itemCarruselActual != value)
                {
                    _itemCarruselActual = value;
                    OnPropertyChanged();
                    OnCarruselSeleccionado(value);
                }
            }
        }

        private int _posicionCarrusel = 0;
        public int PosicionCarrusel
        {
            get => _posicionCarrusel;
            set
            {
                if (_posicionCarrusel != value)
                {
                    _posicionCarrusel = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _vistaPreviaSubtitulo = "Bizcocho de Chocolate con Cobertura Ganache";
        public string VistaPreviaSubtitulo
        {
            get => _vistaPreviaSubtitulo;
            set
            {
                if (_vistaPreviaSubtitulo != value)
                {
                    _vistaPreviaSubtitulo = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _colorAcentoActual = "#5D4037";
        public string ColorAcentoActual
        {
            get => _colorAcentoActual;
            set
            {
                if (_colorAcentoActual != value)
                {
                    _colorAcentoActual = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand GuardarPedidoCommand { get; }
        public ICommand SeleccionarSaborRapidoCommand { get; }

        public PedidoPersonalizadoViewModel()
        {
            _pastelService = new PastelApiService();
            _transaccionesService = new TransaccionesService();
            GuardarPedidoCommand = new Command(async () => await GuardarPedido());
            SeleccionarSaborRapidoCommand = new Command<object>(param => SeleccionarSaborRapido(param));

            InicializarCarrusel();
            CargarCatalogos();
        }

        private void InicializarCarrusel()
        {
            var presets = new List<PastelVisualItem>
            {
                new PastelVisualItem
                {
                    Id = 1,
                    Nombre = "Chocolate Extremo & Ganache",
                    BizcochoNombre = "Chocolate",
                    GlaseadoNombre = "Ganache de Chocolate",
                    RellenoSugerido = "Nutella",
                    FotoUrl = "https://images.unsplash.com/photo-1578985545062-69928b1d9587",
                    Descripcion = "Bizcocho húmedo de chocolate oscuro bañado con ganache brillante y virutas de cacao.",
                    ColorAcento = "#5D4037",
                    Emoji = "🍫",
                    CategoriaId = 1
                },
                new PastelVisualItem
                {
                    Id = 2,
                    Nombre = "Red Velvet Real & Queso Crema",
                    BizcochoNombre = "Red Velvet",
                    GlaseadoNombre = "Queso Crema",
                    RellenoSugerido = "Mermelada de Frambuesa",
                    FotoUrl = "https://images.unsplash.com/photo-1586788680434-30d324b2d46f",
                    Descripcion = "Terciopelo rojo clásico con notas avainilladas y suave glaseado cremoso.",
                    ColorAcento = "#C62828",
                    Emoji = "❤️",
                    CategoriaId = 1
                },
                new PastelVisualItem
                {
                    Id = 3,
                    Nombre = "Vainilla Francesa & Chantilly",
                    BizcochoNombre = "Vainilla",
                    GlaseadoNombre = "Chantilly",
                    RellenoSugerido = "Fresa",
                    FotoUrl = "https://images.unsplash.com/photo-1464349095431-e9a21285b5f3",
                    Descripcion = "Bizcocho dorado de vainilla natural cubierto de crema chantilly ligera y frutos frescos.",
                    ColorAcento = "#8D6E63",
                    Emoji = "🍓",
                    CategoriaId = 4
                },
                new PastelVisualItem
                {
                    Id = 4,
                    Nombre = "Zanahoria Rústica & Especias",
                    BizcochoNombre = "Zanahoria",
                    GlaseadoNombre = "Queso Crema",
                    RellenoSugerido = "Crema Pastelera",
                    FotoUrl = "https://images.unsplash.com/photo-1621303837174-89787a7d4729",
                    Descripcion = "Bizcocho especiado con canela y nuez pecana, cubierto con generoso queso crema.",
                    ColorAcento = "#E65100",
                    Emoji = "🥕",
                    CategoriaId = 8
                },
                new PastelVisualItem
                {
                    Id = 5,
                    Nombre = "Limón Cítrico & Merengue",
                    BizcochoNombre = "Limón",
                    GlaseadoNombre = "Merengue Italiano",
                    RellenoSugerido = "Crema de Limón",
                    FotoUrl = "https://images.unsplash.com/photo-1519869325930-281384150729",
                    Descripcion = "Bizcocho refrescante infusionado en limón eureka con copetes de merengue tostado.",
                    ColorAcento = "#F57F17",
                    Emoji = "🍋",
                    CategoriaId = 1
                },
                new PastelVisualItem
                {
                    Id = 6,
                    Nombre = "Mármol Suizo & Buttercream",
                    BizcochoNombre = "Mármol",
                    GlaseadoNombre = "Crema de Mantequilla",
                    RellenoSugerido = "Cajeta",
                    FotoUrl = "https://images.unsplash.com/photo-1535141192574-5d4897c12636",
                    Descripcion = "Fusión armoniosa de vainilla y cacao con sedosa crema de mantequilla artesanal.",
                    ColorAcento = "#6D4C41",
                    Emoji = "☕",
                    CategoriaId = 4
                },
                new PastelVisualItem
                {
                    Id = 7,
                    Nombre = "Naranja Silvestre & Fondant",
                    BizcochoNombre = "Naranja",
                    GlaseadoNombre = "Fondant",
                    RellenoSugerido = "Durazno",
                    FotoUrl = "https://images.unsplash.com/photo-1550617931-e17a7b70dce2",
                    Descripcion = "Sabor cítrico delicado con un acabado elegante y pulido en fondant dulce.",
                    ColorAcento = "#EF6C00",
                    Emoji = "🍊",
                    CategoriaId = 7
                },
                new PastelVisualItem
                {
                    Id = 8,
                    Nombre = "Tres Leches & Canela",
                    BizcochoNombre = "Esponja",
                    GlaseadoNombre = "Chantilly",
                    RellenoSugerido = "Crema Pastelera",
                    FotoUrl = "https://images.unsplash.com/photo-1565958011703-44f9829ba187",
                    Descripcion = "Esponja aireada empapada en tres leches dulces con copete de crema y canela.",
                    ColorAcento = "#A1887F",
                    Emoji = "🥛",
                    CategoriaId = 4
                }
            };

            foreach (var preset in presets)
            {
                SaboresCarrusel.Add(preset);
            }

            _itemCarruselActual = SaboresCarrusel.FirstOrDefault();
        }

        private async void CargarCatalogos()
        {
            var bizcos = await _pastelService.GetBizcochosAsync();
            foreach (var b in bizcos) Bizcochos.Add(b);

            var rels = await _pastelService.GetRellenosAsync();
            foreach (var r in rels) Rellenos.Add(r);

            var glas = await _pastelService.GetGlaseadosAsync();
            foreach (var g in glas) Glaseados.Add(g);

            var cats = await _pastelService.GetCategoriasAsync();
            foreach (var c in cats) Categorias.Add(c);

            // Sincronizar selección inicial
            if (ItemCarruselActual != null)
            {
                SincronizarFormularioConItem(ItemCarruselActual);
            }
        }

        private void SeleccionarSaborRapido(object? param)
        {
            if (param is PastelVisualItem visualItem)
            {
                ItemCarruselActual = visualItem;
                int index = SaboresCarrusel.IndexOf(visualItem);
                if (index >= 0)
                {
                    PosicionCarrusel = index;
                }
            }
            else if (param is string saborNombre && !string.IsNullOrWhiteSpace(saborNombre))
            {
                var match = SaboresCarrusel.FirstOrDefault(s => 
                    string.Equals(s.BizcochoNombre, saborNombre, StringComparison.OrdinalIgnoreCase) ||
                    s.Nombre.Contains(saborNombre, StringComparison.OrdinalIgnoreCase));

                if (match != null)
                {
                    ItemCarruselActual = match;
                    int index = SaboresCarrusel.IndexOf(match);
                    if (index >= 0)
                    {
                        PosicionCarrusel = index;
                    }
                }
            }
        }

        private void OnCarruselSeleccionado(PastelVisualItem? item)
        {
            if (item == null || _isSyncing) return;
            _isSyncing = true;
            try
            {
                SincronizarFormularioConItem(item);
            }
            finally
            {
                _isSyncing = false;
            }
        }

        private void SincronizarFormularioConItem(PastelVisualItem item)
        {
            FotoUrl = item.FotoUrl;
            Nombre = item.Nombre;
            ColorAcentoActual = item.ColorAcento;
            VistaPreviaSubtitulo = $"{item.BizcochoNombre} • {item.GlaseadoNombre}";

            // Sincronizar Bizcocho
            var biz = Bizcochos.FirstOrDefault(b => string.Equals(b.Nombre, item.BizcochoNombre, StringComparison.OrdinalIgnoreCase));
            if (biz != null)
            {
                _bizcochoSeleccionado = biz;
                OnPropertyChanged(nameof(BizcochoSeleccionado));
            }

            // Sincronizar Glaseado (Cobertura)
            var gla = Glaseados.FirstOrDefault(g => string.Equals(g.Nombre, item.GlaseadoNombre, StringComparison.OrdinalIgnoreCase));
            if (gla != null)
            {
                _glaseadoSeleccionado = gla;
                OnPropertyChanged(nameof(GlaseadoSeleccionado));
            }

            // Sincronizar Relleno sugerido si está disponible
            if (!string.IsNullOrEmpty(item.RellenoSugerido))
            {
                var rel = Rellenos.FirstOrDefault(r => string.Equals(r.Nombre, item.RellenoSugerido, StringComparison.OrdinalIgnoreCase));
                if (rel != null)
                {
                    _rellenoSeleccionado = rel;
                    OnPropertyChanged(nameof(RellenoSeleccionado));
                }
            }

            // Sincronizar Categoría por defecto si está disponible
            var cat = Categorias.FirstOrDefault(c => c.Id == item.CategoriaId);
            if (cat != null)
            {
                _categoriaSeleccionada = cat;
                OnPropertyChanged(nameof(CategoriaSeleccionada));
            }
        }

        private void OnBizcochoOGlaseadoCambiado()
        {
            if (_isSyncing) return;

            string bizNombre = BizcochoSeleccionado?.Nombre ?? string.Empty;
            string glaNombre = GlaseadoSeleccionado?.Nombre ?? string.Empty;

            if (string.IsNullOrEmpty(bizNombre) && string.IsNullOrEmpty(glaNombre)) return;

            // Buscar si existe un item exacto en el carrusel
            var exacto = SaboresCarrusel.FirstOrDefault(s =>
                string.Equals(s.BizcochoNombre, bizNombre, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(s.GlaseadoNombre, glaNombre, StringComparison.OrdinalIgnoreCase));

            if (exacto != null)
            {
                _isSyncing = true;
                ItemCarruselActual = exacto;
                int idx = SaboresCarrusel.IndexOf(exacto);
                if (idx >= 0) PosicionCarrusel = idx;
                FotoUrl = exacto.FotoUrl;
                ColorAcentoActual = exacto.ColorAcento;
                VistaPreviaSubtitulo = $"{bizNombre} • {glaNombre}";
                _isSyncing = false;
                return;
            }

            // Si no hay match exacto de ambos, buscar coincidencia por sabor de bizcocho
            var porBizcocho = SaboresCarrusel.FirstOrDefault(s =>
                string.Equals(s.BizcochoNombre, bizNombre, StringComparison.OrdinalIgnoreCase));

            if (porBizcocho != null)
            {
                _isSyncing = true;
                int idx = SaboresCarrusel.IndexOf(porBizcocho);
                if (idx >= 0) PosicionCarrusel = idx;
                _isSyncing = false;
            }

            // Resolver imagen dinámica representativa de la combinación
            FotoUrl = ResolverImagenDinamica(bizNombre, glaNombre);
            ColorAcentoActual = porBizcocho?.ColorAcento ?? "#8B5E34";
            VistaPreviaSubtitulo = $"{bizNombre} con Cobertura de {glaNombre}";
        }

        private string ResolverImagenDinamica(string bizcocho, string glaseado)
        {
            // Mapeo dinámico e inteligente de imágenes de alta fidelidad según sabor y cobertura
            string key = $"{bizcocho}_{glaseado}".ToLowerInvariant();

            if (key.Contains("chocolate") && (key.Contains("ganache") || key.Contains("espejo")))
                return "https://images.unsplash.com/photo-1578985545062-69928b1d9587";
            if (key.Contains("chocolate"))
                return "https://images.unsplash.com/photo-1606890737304-57a1ca8a5b62";
            if (key.Contains("red velvet"))
                return "https://images.unsplash.com/photo-1586788680434-30d324b2d46f";
            if (key.Contains("zanahoria"))
                return "https://images.unsplash.com/photo-1621303837174-89787a7d4729";
            if (key.Contains("limón") || key.Contains("limon"))
                return "https://images.unsplash.com/photo-1519869325930-281384150729";
            if (key.Contains("naranja"))
                return "https://images.unsplash.com/photo-1550617931-e17a7b70dce2";
            if (key.Contains("mármol") || key.Contains("marmol"))
                return "https://images.unsplash.com/photo-1535141192574-5d4897c12636";
            if (key.Contains("esponja") || key.Contains("tres leches"))
                return "https://images.unsplash.com/photo-1565958011703-44f9829ba187";
            if (key.Contains("fondant"))
                return "https://images.unsplash.com/photo-1535254973040-607b474cb50d";
            if (key.Contains("merengue"))
                return "https://images.unsplash.com/photo-1519869325930-281384150729";
            if (key.Contains("queso"))
                return "https://images.unsplash.com/photo-1533134242443-d4fd215305ad";
            
            // Imagen de vainilla gourmet por defecto
            return "https://images.unsplash.com/photo-1464349095431-e9a21285b5f3";
        }

        private async Task GuardarPedido()
        {
            if (string.IsNullOrWhiteSpace(Nombre) || 
                BizcochoSeleccionado == null || 
                RellenoSeleccionado == null || 
                GlaseadoSeleccionado == null || 
                CategoriaSeleccionada == null || 
                string.IsNullOrWhiteSpace(Tamanio) || 
                string.IsNullOrWhiteSpace(Cantidad))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Por favor, completa todos los campos requeridos.", "OK");
                return;
            }

            if (!int.TryParse(Cantidad, out int qty) || qty <= 0)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "La cantidad debe ser un número válido mayor a 0.", "OK");
                return;
            }

            var nuevoPastel = new Pastel
            {
                Nombre = Nombre,
                Tamanio = Tamanio,
                FotoUrl = string.IsNullOrWhiteSpace(FotoUrl) ? "https://images.unsplash.com/photo-1578985545062-69928b1d9587" : FotoUrl,
                BizcochoId = BizcochoSeleccionado.Id,
                RellenoId = RellenoSeleccionado.Id,
                GlaseadoId = GlaseadoSeleccionado.Id,
                CategoriaId = CategoriaSeleccionada.Id,
                IsPersonalizado = true
            };

            var pastelCreado = await _pastelService.RegistrarPastelAsync(nuevoPastel);
            if (pastelCreado != null)
            {
                var nuevoPedido = new Pedido
                {
                    PastelId = pastelCreado.Id,
                    UsuarioId = AppSession.CurrentUser?.Id ?? 2,
                    Cantidad = qty,
                    FechaEntrega = FechaEntrega
                };

                var exito = await _transaccionesService.CrearPedidoAsync(nuevoPedido);
                if (exito)
                {
                    await Application.Current.MainPage.DisplayAlert("¡Pedido Exitoso!", 
                        $"Tu orden artesanal para '{Nombre}' ha sido creada exitosamente.\n" +
                        $"Sabor: {BizcochoSeleccionado.Nombre}\n" +
                        $"Cobertura: {GlaseadoSeleccionado.Nombre}\n" +
                        $"Entrega: {FechaEntrega:dd/MM/yyyy}", "Aceptar");
                    await Shell.Current.Navigation.PopAsync();
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "El pastel se creó, pero falló el registro del pedido.", "OK");
                }
            }
            else
            {
                if (!AppSession.HasAuthError)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "No se pudo registrar tu pastel personalizado.", "OK");
                }
                AppSession.HasAuthError = false;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
