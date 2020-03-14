using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


namespace API.Controllers
{ 
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriaController : ControllerBase
    {
        private CategoriaDAO categoriaDAO = new CategoriaDAO();
        private readonly ILogger<CategoriaController> logger;

        public CategoriaController(ILogger<CategoriaController> logger)
        {
            this.logger = logger;
        }


        [HttpGet]
        [Route("Todas")]
        public IList<Categoria> GetAllCategorias()
        {
            return null;
        }



        [HttpPost]
        public void AddCategoria(string Nome)
        {
            if (categoriaDAO.ContainsNome(Nome))
            {

            }
            else
            {
                categoriaDAO.AddNovaCategoria(Nome);

            }
        }
    }
}
