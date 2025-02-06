using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal interface IDamageable
{
    public void TakeDamage(int damage);
    public void Die();
}
