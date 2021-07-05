using System.Threading.Tasks;
using dotnet_rpg.Controllers.Dtos.Character;
using dotnet_rpg.Dtos.Weapon;
using dotnet_rpg.Models;
using dotnet_rpg.Services.WeaponService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_rpg.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class WeaponController : ControllerBase
    {   
        public IWeaponService WeaponService;
        public WeaponController(IWeaponService weaponService)
        {
            this.WeaponService = weaponService;
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<GetCharacterDto>>> AddWeapon(AddWeaponDto newWeapon){
            return Ok(await WeaponService.AddWeapon(newWeapon));
        }
    }
}