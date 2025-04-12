using System.Collections.Generic;

public class SequenceNode : Node
{
    private List<Node> children = new List<Node>(); // 🔥 Thêm danh sách children
    private int currentTaskIndex = 0; // Chỉ số của Task hiện tại

    public void AddChild(Node child) => children.Add(child);

    public override NodeState Evaluate(BlackboardBase blackboard)
    {
        if (currentTaskIndex >= children.Count)
        {
            currentTaskIndex = 0; // Reset lại sau khi hoàn thành tất cả task
            return NodeState.SUCCESS;
        }

        NodeState result = children[currentTaskIndex].Evaluate(blackboard);

        if (result == NodeState.SUCCESS)
        {
            currentTaskIndex++; // Chuyển sang Task tiếp theo nếu thành công
            return NodeState.RUNNING; // Đánh dấu rằng Sequence vẫn đang chạy
        }

        if (result == NodeState.FAILURE)
        {
            currentTaskIndex = 0; // Reset nếu có Task thất bại
            return NodeState.FAILURE;
        }

        return NodeState.RUNNING; // Nếu Task đang chạy, giữ nguyên
    }
}
