using DataAccessLayer.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers { 
[Route("api/[controller]")]
[ApiController]
public class EncryptionController : ControllerBase
{
    public readonly ICS _aesHelper;
    public EncryptionController(ICS iCS) {
            _aesHelper = iCS;
        }

    [HttpPost("encrypt")]
    public IActionResult Encrypt([FromBody] string plainText)
    {
        try
        {
            if (string.IsNullOrEmpty(plainText))
                return BadRequest("Input text cannot be empty.");
                string ciphertext = _aesHelper.Encrypt(plainText);

                
            return Ok(new { EncryptedText = ciphertext });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpPost("decrypt")]
    public IActionResult Decrypt([FromBody] string cipherText)
    {
        try
        {
            if (string.IsNullOrEmpty(cipherText))
                return BadRequest("Input text cannot be empty.");

            string decryptedText = _aesHelper.Decrypt(cipherText);
            return Ok(new { DecryptedText = decryptedText });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }
}
}
