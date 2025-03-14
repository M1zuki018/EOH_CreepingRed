using System.Collections.Generic;
using System.Drawing;

/// <summary>
/// クアッドツリー
/// </summary>
public class Quadtree
{
    private int _capacity; // 容量の上限
    private List<Agent> _agents;
    private Rectangle _boundary; // 境界
    private bool _divided = false;
    private Quadtree _northwest, _northeast;

    public Quadtree(Rectangle boundary, int capacity)
    {
        _boundary = boundary;
        _capacity = capacity;
        _agents = new List<Agent>();
    }

    /// <summary>
    /// 処理が必要なエリアを取得する
    /// </summary>
    public List<Agent> GetInfectedAreas()
    {
        return _agents;
    }

    /// <summary>
    /// 感染状況を更新する
    /// </summary>
    public void UpdateInfectedStatus(List<Agent> agents)
    {
        _agents = agents;
    }
    
    public bool Insert(Agent agent)
    {
        if (!_boundary.Contains(agent.X, agent.Y))
        {
            // 範囲外なら無視
            return false;
        }

        if (_agents.Count < _capacity)
        {
            // 容量に空きがあれば追加
            _agents.Add(agent);
            return true;
        }

        if (!_divided)
        {
            // 分割して再配置を行う
            Subdivide();
        }
        
        return (_northwest.Insert(agent) || _northeast.Insert(agent));
    }
    
    /// <summary>
    /// 細分化処理
    /// </summary>
    private void Subdivide() {
        int x = _boundary.X, y = _boundary.Y;
        int w = _boundary.Width / 2, h = _boundary.Height / 2;

        _northwest = new Quadtree(new Rectangle(x, y, w, h), _capacity);
        _northeast = new Quadtree(new Rectangle((x + w), y, w, h), _capacity);
        
        _divided = true;
    }
    /*
    public List<Agent> Query(Rectangle range, List<Agent> found) 
    {
        if (!_boundary.Intersects(range)) return found;

        foreach (var agent in _agents) {
            if (range.Contains(agent.X, agent.Y)) {
                found.Add(agent);
            }
        }

        if (_divided) {
            _northwest.Query(range, found);
            _northeast.Query(range, found);
        }

        return found;
    }
    */
}
