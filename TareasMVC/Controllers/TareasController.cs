using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TareasMVC.Entidades;
using TareasMVC.Models;
using TareasMVC.Servicios;

namespace TareasMVC.Controllers
{
    [Route("api/tareas")]
    public class TareasController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IServicioUsuarios servicioUsuarios;
        private readonly IMapper mapper;

        public TareasController(ApplicationDbContext context,
            IServicioUsuarios servicioUsuarios,
            IMapper mapper)
        {
            this.context = context;
            this.servicioUsuarios = servicioUsuarios;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<TareaDTO>>> Get()
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tareas = await context.Tareas
                .Where(t => t.UsuarioCreacionId == usuarioId && t.TareaPadreId == null)
                .OrderBy(t => t.Orden)
                .ProjectTo<TareaDTO>(mapper.ConfigurationProvider)
                .ToListAsync();

            return tareas;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Tarea>> Get(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

            var tarea = await context.Tareas
                .Include(t => t.Pasos.OrderBy(p => p.Orden))
                .Include(t => t.ArchivosAdjuntos.OrderBy(a => a.Orden))
                .Include(t => t.Subtareas.OrderBy(s => s.Orden))
                .FirstOrDefaultAsync(t => t.Id == id && t.UsuarioCreacionId == usuarioId);

            if (tarea is null)
            {
                return NotFound();
            }

            return tarea;
        }

        [HttpPost]
        public async Task<ActionResult<Tarea>> Post([FromBody] string titulo)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

            var existenTareas = await context.Tareas.AnyAsync(t => t.UsuarioCreacionId == usuarioId);

            var ordenMayor = 0;
            if (existenTareas)
            {
                ordenMayor = await context.Tareas.Where(t => t.UsuarioCreacionId == usuarioId)
                    .Select(t => t.Orden).MaxAsync();
            }

            var tarea = new Tarea
            {
                Titulo = titulo,
                UsuarioCreacionId = usuarioId,
                FechaCreacion = DateTime.UtcNow,
                Orden = ordenMayor + 1
            };

            context.Add(tarea);
            await context.SaveChangesAsync();

            return tarea;
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> EditarTarea(int id, [FromBody] TareaEditarDTO tareaEditarDTO)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

            var tarea = await context.Tareas.FirstOrDefaultAsync(t => t.Id == id &&
            t.UsuarioCreacionId == usuarioId);

            if (tarea is null)
            {
                return NotFound();
            }

            tarea.Titulo = tareaEditarDTO.Titulo;
            tarea.Descripcion = tareaEditarDTO.Descripcion;

            await context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

            var tarea = await context.Tareas.FirstOrDefaultAsync(t => t.Id == id &&
                t.UsuarioCreacionId == usuarioId);

            if (tarea is null)
            {
                return NotFound();
            }

            tarea.FechaEliminacion = DateTime.UtcNow;
            await context.SaveChangesAsync();
            return Ok();
        }


        [HttpPost("ordenar")]
        public async Task<IActionResult> Ordenar([FromBody] int[] ids)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

            var tareas = await context.Tareas
                .Where(t => t.UsuarioCreacionId == usuarioId).ToListAsync();

            var tareasId = tareas.Select(t => t.Id);

            var idsTareasNoPertenecenAlUsuario = ids.Except(tareasId).ToList();

            if (idsTareasNoPertenecenAlUsuario.Any())
            {
                return Forbid();
            }

            var tareasDiccionario = tareas.ToDictionary(x => x.Id);

            for (int i = 0; i < ids.Length; i++)
            {
                var id = ids[i];
                var tarea = tareasDiccionario[id];
                tarea.Orden = i + 1;
            }

            await context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("{tareaPadreId:int}/subtareas")]
        public async Task<ActionResult<Tarea>> PostSubtarea(int tareaPadreId, [FromBody] string titulo)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

            var tareaPadre = await context.Tareas.FirstOrDefaultAsync(t => t.Id == tareaPadreId &&
                t.UsuarioCreacionId == usuarioId);

            if (tareaPadre is null)
            {
                return NotFound();
            }

            var existenSubtareas = await context.Tareas.AnyAsync(t => t.TareaPadreId == tareaPadreId);

            var ordenMayor = 0;
            if (existenSubtareas)
            {
                ordenMayor = await context.Tareas.Where(t => t.TareaPadreId == tareaPadreId)
                    .Select(t => t.Orden).MaxAsync();
            }

            var subtarea = new Tarea
            {
                Titulo = titulo,
                UsuarioCreacionId = usuarioId,
                FechaCreacion = DateTime.UtcNow,
                Orden = ordenMayor + 1,
                TareaPadreId = tareaPadreId
            };

            context.Add(subtarea);
            await context.SaveChangesAsync();
            return subtarea;
        }

        [HttpGet("papelera")]
        public async Task<ActionResult<List<TareaDTO>>> GetPapelera()
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

            var tareas = await context.Tareas
                .IgnoreQueryFilters()
                .Where(t => t.UsuarioCreacionId == usuarioId && t.FechaEliminacion != null)
                .OrderByDescending(t => t.FechaEliminacion)
                .ProjectTo<TareaDTO>(mapper.ConfigurationProvider)
                .ToListAsync();

            return tareas;
        }

        [HttpPost("{id:int}/restaurar")]
        public async Task<ActionResult> Restaurar(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

            var tarea = await context.Tareas
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(t => t.Id == id && t.UsuarioCreacionId == usuarioId);

            if (tarea is null)
            {
                return NotFound();
            }

            tarea.FechaEliminacion = null;
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id:int}/permanente")]
        public async Task<ActionResult> DeletePermanente(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

            var tarea = await context.Tareas
                .IgnoreQueryFilters()
                .Include(t => t.Subtareas)
                .FirstOrDefaultAsync(t => t.Id == id && t.UsuarioCreacionId == usuarioId);

            if (tarea is null)
            {
                return NotFound();
            }

            foreach (var subtarea in tarea.Subtareas)
            {
                subtarea.TareaPadreId = null;
            }

            context.Remove(tarea);
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}
