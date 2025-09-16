using API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public sealed class GeneratesController(NovelAiDbContext context) : APIConrollerBase
{
    [HttpPost]
    public async Task<ActionResult<string>> Generate([FromBody] GenerateContent content)
    {
        if (content is null) return BadRequest("Empty request.");

        // wczytaj projekt z nawigacjami
        var project = await context.Projects
            .Include(p => p.User)
            .ThenInclude(u => u.WriteSettings)
            .FirstOrDefaultAsync(p => p.Id == content.ProjectId);

        if (project is null) return NotFound("Project not found.");
        if (project.User?.WriteSettings is null) return BadRequest("Missing WriteSettings for project owner.");

        var ws = project.User.WriteSettings;

        // domyślne progi jeśli per-request null/0
        var nsfwReq = content.NSFWThreshold is > 0 ? content.NSFWThreshold : ws.NSFWThreshold;
        var ecchiReq = content.EcchiThreshold is > 0 ? content.EcchiThreshold : ws.EcchiThreshold;
        var goreReq = content.GoreThreshold is > 0 ? content.GoreThreshold : ws.GoreThreshold;

        // global
        var globalPrompt = string.Format(UtilsPrompts.GLOBAL_PROMPT,
            ws.Language ?? "English",
            ws.Ecchi ?? "tasteful erotic tension: nudity, teasing, suggestive situations, fanservice; no explicit sex",
            ws.NSFW ?? "explicit sexual content as literary, aesthetic prose; not mechanical/clinical",
            ws.Gore ?? "graphic violence only when narratively meaningful",
            ws.NSFWThreshold.ToString("0.00"),
            ws.EcchiThreshold.ToString("0.00"),
            ws.GoreThreshold.ToString("0.00"));

        // user
        var userPrompt = string.Format(UtilsPrompts.USER_PROMPT,
            content.Prompt ?? "Write a scene.",
            content.MinWordLength > 0 ? content.MinWordLength : 250,
            content.MaxWordLength > 0 ? content.MaxWordLength : 900,
            nsfwReq.ToString("0.00"),
            ecchiReq.ToString("0.00"),
            goreReq.ToString("0.00"));

        // format wyjścia
        var finalPrompt = globalPrompt + "\n\n" + userPrompt + "\n\n" + UtilsPrompts.OUTPUT_JSON_INSTRUCTION;

        // TODO: wyślij finalPrompt do modelu AI i odbierz JSON.
        // Pseudokod:
        // var raw = await aiClient.GenerateAsync(finalPrompt, ct);
        // var resp = JsonSerializer.Deserialize<AiResponse>(raw, new JsonSerializerOptions{ PropertyNameCaseInsensitive = true });
        // if (resp is null) return StatusCode(502, "AI returned invalid JSON.");
        // return Ok(resp.content);

        // for now return prompt (dev only):
        return Ok(finalPrompt);
    }

    #region HelperClasses

    public class GenerateContent
    {
        public int ProjectId { get; set; }
        public string? Prompt { get; set; }

        public int MinWordLength { get; set; } 
        public int MaxWordLength { get; set; }

        public float NSFWThreshold { get; set; } // 0..1 
        public float EcchiThreshold { get; set; } // 0..1
        public float GoreThreshold { get; set; } // 0..1
    }

    #endregion HelperClasses
}