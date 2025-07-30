namespace AutoShop.Models
{
    // ����� �� ��������� �� ���������� ��� ������ � ������������
    public class ErrorViewModel
    {
        // ������������� �� �������� ������, ����� ������ ��� ������������ �� ������ � �������
        public string? RequestId { get; set; }

        // ����� true, ��� RequestId �� � null ��� ������ � ���� ����� �� ��������� �� RequestId ��� view-��
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
