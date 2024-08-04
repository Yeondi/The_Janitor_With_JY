public interface IAttackable
{
    int AttackPower { get; set; }
    void Attack(Character target);
    void TakeDamage(int damage);

    void Die();
}