using LogisticsEngine.Domain.Hubs.Entities;
using LogisticsEngine.Domain.Hubs.Errors;
using LogisticsEngine.Domain.Hubs.Vo;

namespace LogisticsEngine.UnitTests;

public sealed class HubTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_rejects_missing_names(string? name) =>
        Hub.Create(Guid.NewGuid(), name, "NORD").Error.ShouldBe(HubErrors.InvalidName);

    [Fact]
    public void Create_checks_id_and_name_length()
    {
        Hub.Create(Guid.Empty, "Milano", "NORD").Error.ShouldBe(HubErrors.InvalidId);
        Hub.Create(Guid.NewGuid(), new string('a', 121), "NORD").Error.ShouldBe(HubErrors.InvalidName);
        Hub.Create(Guid.NewGuid(), "A", "NORD").IsSuccess.ShouldBeTrue();
    }


    [Fact]
    public void Create_preserves_id_and_normalizes_values()
    {
        Guid id = Guid.NewGuid();
        Hub hub = Hub.Create(id, " Milano ", " nord ").Value;
        hub.Id.ShouldBe(id);
        hub.Name.ShouldBe("Milano");
        hub.Zone.Code.ShouldBe("NORD");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("A")]
    [InlineData("ABCDEFGHIJK")]
    [InlineData("N0RD")]
    [InlineData("N ORD")]
    [InlineData("SÙD")]
    public void Invalid_zones_are_rejected_by_zone_and_hub(string? code)
    {
        Zone.Create(code).Error.ShouldBe(HubErrors.InvalidZone);
        Hub.Create(Guid.NewGuid(), "Milano", code).Error.ShouldBe(HubErrors.InvalidZone);
    }

    [Theory]
    [InlineData("AB")]
    [InlineData("ABCDEFGHIJ")]
    [InlineData(" nord ")]
    public void Zone_has_value_equality(string code)
    {
        Zone first = Zone.Create(code).Value;
        Zone second = Zone.Create(code.Trim().ToUpperInvariant()).Value;
        first.Equals(second).ShouldBeTrue();
        first.GetHashCode().ShouldBe(second.GetHashCode());
        first.ToString().ShouldBe(first.Code);
        first.Equals(null).ShouldBeFalse();
        first.Equals("NORD").ShouldBeFalse();
        first.Equals(Zone.Create("SUD").Value).ShouldBeFalse();
    }

    [Theory]
    [InlineData("", "SUD", "Hub.InvalidName")]
    [InlineData("Roma", "1", "Hub.InvalidZone")]
    public void Failed_update_does_not_change_any_state(string name, string zone, string error)
    {
        Hub hub = Hub.Create(Guid.NewGuid(), "Milano", "NORD").Value;
        hub.Update(name, zone).Error.Code.ShouldBe(error);
        hub.Name.ShouldBe("Milano");
        hub.Zone.Code.ShouldBe("NORD");
    }

    [Fact]
    public void Update_changes_values_but_preserves_identity()
    {
        Guid id = Guid.NewGuid();
        Hub hub = Hub.Create(id, "Milano", "NORD").Value;
        hub.Update(" Roma ", " centro ").IsSuccess.ShouldBeTrue();
        hub.Id.ShouldBe(id);
        hub.Name.ShouldBe("Roma");
        hub.Zone.Code.ShouldBe("CENTRO");
    }
}
