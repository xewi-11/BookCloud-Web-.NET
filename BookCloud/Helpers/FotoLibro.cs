using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using static BookCloud.Helpers.FolderHelper;

namespace BookCloud.Helpers
{

    public class FotoLibro
    {
        private IWebHostEnvironment webHostEnvironment;
        private IServer server;
        public FotoLibro(IWebHostEnvironment webHostEnvironment, IServer server)
        {
            this.webHostEnvironment = webHostEnvironment;
            this.server = server;
        }
        public string MapPath(string fileName, Folder folder, int userId, string titulo)
        {
            if (folder == Folder.Libros)
            {
                string carpeta = "libros";
                string rootPath = this.webHostEnvironment.WebRootPath;
                string fileNameWithId = $"{userId}_{titulo}_{fileName}";
                return Path.Combine(rootPath, "imagenes", carpeta, fileNameWithId);
            }
            else
            {
                return "Folder no reconocido";
            }
        }
        public string MapUrlPath(string fileName, Folder folder, int userId)
        {
            if (folder == Folder.Libros)
            {
                string carpeta = "libros";
                var adresses = this.server.Features.Get<IServerAddressesFeature>().Addresses;
                string serverUrl = adresses.FirstOrDefault()?.ToString();
                return $"{serverUrl}/imagenes/{carpeta}/{fileName}";
            }
            else
            {
                return "Folder no reconocido";
            }
        }
    }
}
