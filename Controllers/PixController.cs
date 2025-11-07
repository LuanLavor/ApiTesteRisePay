using Microsoft.AspNetCore.Mvc;
using ApiTesteRisePay.Classes;
using System.Threading.Tasks;

[ApiController]
[Route("[controller]")]
public class PixController : ControllerBase
{
    private readonly RisePayService _risePayService;

    public PixController(RisePayService risePayService)
    {
        _risePayService = risePayService;
    }

    [HttpPost("gerar")]
    public async Task<IActionResult> GerarPix([FromBody] decimal amount)
    {
        try
        {

            var pixResponse = await _risePayService.GerarPixAsync(amount);

            var responseDto = new PixResponseDto
            {
                QrCode = pixResponse.qrCode,
                Value = pixResponse.value,
                QrCodeImage = RisePayService.QrCodeHelper.GerarQrCodeBase64(pixResponse.qrCode)
            };

            return Ok(responseDto);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
