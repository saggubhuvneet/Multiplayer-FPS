using Photon.Pun;

public interface IDamageable
{
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info);
    void TakeDamage(float damage);
}