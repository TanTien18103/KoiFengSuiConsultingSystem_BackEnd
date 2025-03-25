using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Constants
{
    public class ApplicationConstants
    {
        public const string KEYID_EXISTED = "KeyId {0} đã tồn tại.";
        public const string KeyId = "KeyId";
        public const string DUPLICATE = "Symtem_id is duplicated";
    }

    //Service (1-0-0)
    public class ResponseCodeConstants
    {
        public const string NOT_FOUND = "Not found!";
        public const string BAD_REQUEST = "Bad request!";
        public const string SUCCESS = "Success!";
        public const string FAILED = "Failed!";
        public const string EXISTED = "Existed!";
        public const string DUPLICATE = "Duplicate!";
        public const string INTERNAL_SERVER_ERROR = "INTERNAL_SERVER_ERROR";
        public const string INVALID_INPUT = "Invalid input!";
        public const string REQUIRED_INPUT = "Input required!";
        public const string UNAUTHORIZED = "Unauthorized!";
        public const string FORBIDDEN = "Forbidden!";
        public const string EXPIRED = "Expired!";
    }

    //Controllers
    public class ResponseMessageConstantsCommon
    {
        public const string NOT_FOUND = "Không tìm thấy dữ liệu";
        public const string EXISTED = "Already existed!";
        public const string SUCCESS = "Thao tác thành công";
        public const string NO_DATA = "Không có dữ liệu trả về";
        public const string SERVER_ERROR = "Lỗi từ phía server vui lòng liên hệ đội ngũ phát triển";
        public const string DATE_WRONG_FORMAT = "Dữ liệu ngày không đúng định dạng yyyy-mm-dd";
        public const string DATA_NOT_ENOUGH = "Dữ liệu đưa vào không đầy đủ";
    }

    //Auth-Account
    public class ResponseMessageIdentity
    {
        public const string INVALID_USER = "Người dùng không tồn tại.";
        public const string UNAUTHENTICATED = "Không xác thực.";
        public const string UNAUTHENTICATED_OR_UNAUTHORIZED = "Người dùng chưa xác thực hoặc không có quyền truy cập.";
        public const string PASSWORD_NOT_MATCH = "Mật khẩu không giống nhau.";
        public const string NEW_PASSWORD_CANNOT_MATCH = "Mật khẩu mới không được trùng với mật khẩu cũ.";
        public const string PASSWORD_WRONG = "Mật khẩu không đúng.";
        public const string OLD_PASSWORD_WRONG = "Mật khẩu cũ không đúng.";
        public const string PASSWORD_INVALID = "Mật khẩu không hợp lệ.";
        public const string EXISTED_USER_NAME = "Người dùng đã tồn tại.";
        public const string EXISTED_ACCOUNT_NO = "Tài khoản ngân hàng đã tồn tại.";
        public const string EXISTED_EMAIL = "Email đã tồn tại.";
        public const string EXISTED_PHONE = "Số điện thoại đã tồn tại.";
        public const string TOKEN_INVALID_OR_EXPIRED = "Token không xác thực hoặc đã hết hạn.";
        public const string TOKEN_INVALID = "Token xác thực không hợp lệ.";
        public const string TOKEN_NOT_SEND = "Token xác thực không được cung cấp";
        public const string GOOGLE_TOKEN_INVALID = "Token Google không hợp lệ.";
        public const string EMAIL_VALIDATED = "Email đã được xác thực.";
        public const string PHONE_VALIDATED = "Số điện thoại đã được xác thực.";
        public const string ROLE_INVALID = "Role không xác thực.";
        public const string CLAIM_NOTFOUND = "Không tìm thấy claim.";
        public const string EXISTED_ROLE = "Role đã tồn tại.";
        public const string INCORRECT_EMAIL = "Email Không tìm thấy";
        public const string ACCOUNT_NOT_FOUND = "Tài khoản Không tìm thấy";

        public const string USERNAME_REQUIRED = "Tên người dùng không được để trống.";
        public const string NAME_REQUIRED = "Tên không được để trống.";
        public const string GENDER_REQUIRED = "Giới tính không được để trống.";
        public const string USERCODE_REQUIRED = "Mã người dùng không được để trống.";
        public const string PASSWORD_REQUIRED = "Mật khẩu không được để trống.";
        public const string PASSSWORD_LENGTH = "Mật khẩu phải có ít nhất 8 ký tự.";
        public const string CONFIRM_PASSWORD_REQUIRED = "Xác nhận mật khẩu không được để trống.";
        public const string EMAIL_REQUIRED = "Email không được để trống.";
        public const string PHONENUMBER_REQUIRED = "Số điện thoại không được để trống.";
        public const string PHONENUMBER_INVALID = "Số điện thoại không hợp lệ.";
        public const string PHONENUMBER_LENGTH = "Số điện thoại phải có chính xác 10 số.";
        public const string ROLES_REQUIRED = "Role không được để trống.";
        public const string USER_NOT_ALLOWED = "Bạn không có quyền truy cập vào mục này.";
        public const string SESSION_NOT_FOUND = "Không tìm thấy session.";
        public const string SESSION_INVALID = "Session không hợp lệ, hãy đăng nhập lại.";
        public const string EMAIL_VALIDATION_REQUIRED = "Vui lòng nhập mã OTP được gửi đến email của bạn để kích hoạt tài khoản.";
    }

    //Auth-Account Controllers
    public class ResponseMessageIdentitySuccess
    {
        public const string REGIST_USER_SUCCESS = "Đăng kí tài khoản thành công! Vui lòng kiểm tra email để xác thực tài khoản.";
        public const string VERIFY_PHONE_SUCCESS = "Xác thực số điện thoại thành công!";
        public const string VERIFY_EMAIL_SUCCESS = "Xác thực email thành công!";
        public const string FORGOT_PASSWORD_SUCCESS = "Yêu cầu đặt lại mật khẩu thành công! Vui lòng kiểm tra email để đặt lại mật khẩu.";
        public const string RESET_PASSWORD_SUCCESS = "Cấp lại mật khẩu thành công!";
        public const string CHANGE_PASSWORD_SUCCESS = "Đổi mật khẩu thành công!";
        public const string RESEND_EMAIL_SUCCESS = "Gửi lại email xác thực thành công!";
        public const string UPDATE_USER_SUCCESS = "Cập nhật thông tin người dùng thành công!";
        public const string DELETE_USER_SUCCESS = "Xóa người dùng thành công!";
        public const string ADD_ROLE_SUCCESS = "Thêm role thành công!";
        public const string UPDATE_ROLE_SUCCESS = "Cập nhật role thành công!";
        public const string DELETE_ROLE_SUCCESS = "Xóa role thành công!";
    }

    //For User (0-1-0)  
    public class ResponseMessageConstantsUser
    {
        public const string GET_USER_INFO_SUCCESS = "Lấy thông tin người dùng thành công";
        public const string USER_NOT_FOUND = "Không tìm thấy người dùng";
        public const string NOT_UPDATED_ELEMENT = "Người dùng chưa cập nhật thông tin mệnh";
        public const string USER_EXISTED = "Người dùng đã tồn tại";
        public const string ADD_USER_SUCCESS = "Thêm người dùng thành công";
        public const string UPDATE_USER_SUCCESS = "Cập nhật người dùng thành công";
        public const string DELETE_USER_SUCCESS = "Xóa người dùng thành công";
        public const string ADMIN_NOT_FOUND = "Không tìm thấy quản trị viên";
        public const string CUSTOMER_NOT_FOUND = "Không tìm thấy khách hàng";
        public const string CUSTOMER_INFO_NOT_FOUND = "Không tìm thấy thông tin khách hàng";
        public const string CUSTOMER_BANK_INFO_NOT_FOUND = "Không tìm thấy thông tin tài khoản ngân hàng của khách hàng";
    }

    //Order-Payment Service
    public class ResponseMessageConstrantsOrder
    {
        public const string NOT_FOUND = "Không tìm thấy đơn hàng";
        public const string NOT_FOUND_PENDING = "Không tìm thấy đơn hàng đang chờ được xử lý";
        public const string FOUND = "Tìm thấy đơn hàng: ";
        public const string INVALID_DATA = "Dữ liệu không hợp lệ";
        public const string NOTALLOWED = "Bạn không được phép xem đơn hàng này";
        public const string CUSTOMER_NOTALLOWED = "Bạn chỉ được phép xem đơn hàng của chính mình";
        public const string STATUS_CHANGE_NOTALLOWED = "Bạn chưa thể thay đổi trạng thái của đơn hàng này";
        public const string ALREADY_PAID = "Đơn hàng đã được thanh toán";
        public const string NOT_PAID = "Đơn hàng chưa được thanh toán";
        public const string PRICE_NOT_FOUND_OR_INVALID = "Không tìm thấy thông tin giá dịch vụ hoặc giá không hợp lệ";
        public const string SERVICE_TYPE_INVALID = "Loại dịch vụ không hợp lệ";
        public const string WEBHOOK_NOT_FOUND = "Dữ liệu Webhook không tìm thấy";
        public const string REQUEST_FAILED_ORDER = "Gửi yêu cầu thất bại cho đơn hàng: ";
        public const string NEED_TO_PAY_SERVICE_NOT_FOUND = "Không tìm thấy dịch vụ cần thanh toán";
        public const string ORDER_STATUS_TO_PAID = "Cập nhật trạng thái đơn hàng thành đã thanh toán thành công";
        public const string ORDER_STATUS_TO_PENDINGCONFIRM = "Cập nhật trạng thái đơn hàng thành chờ xác nhận đã thanh toán thành công";
        public const string ORDER_CANCELED_SUCCESS = "Đơn hàng được hủy thành công";
        public const string ORDER_EXPIRED = "Đơn hàng đã quá hạn thanh toán";
        public const string SERVICETYPE_INVALID = "Loại dịch vụ không hợp lệ";
        public const string NOT_WAITING_FOR_REFUND = "Đơn hàng này không ở trạng thái Chờ hoàn tiền";
        public const string ORDER_AMOUNT_INVALID = "Đơn hàng này không có giá trị để hoàn tiền";
        public const string CANT_REFUND_FOR_OFFLINE = "Không thể hoàn tiền cho đơn đặt lịch offline";
        public const string WORKSHOP_EXPIRED = "Không thể hủy đơn vì workshop đã bắt đầu hoặc đã kết thúc";
        public const string ONLINE_EXPIRED = "Không thể hủy đơn vì đã đến hoặc quá thời gian đặt lịch";
        public const string COURSE_CONFIRMED = "Không thể hủy đơn vì khóa học đã được đăng ký và được xác nhận";
        public const string WRONG_ORDER = "Đơn hàng hiện tại không thuộc về khách hàng này";

    }

    public class ResponseMessageConstrantsBooking
    {
        public const string BOOKING_CREATED = "Tạo lịch tư vấn thành công";
        public const string ONLINE_GET_SUCCESS = "Lấy thông tin booking online thành công";
        public const string OFFLINE_GET_SUCCESS = "Lấy thông tin booking offline thành công";
        public const string ASSIGNED_SUCCESS = "Gán Master cho Booking thành công";
        public const string NOT_FOUND = "Không tìm thấy lịch tư vấn";
        public const string ALREADY_ASSIGNED = "Lịch tư vấn này đã có Master";
        public const string NOT_FOUND_STATUS = "Không tìm thấy lịch tư vấn với trạng thái ";
        public const string REQUIRED_DATA = "Dữ liệu booking không được để trống";
        public const string INVALID_DATA = "Dữ liệu lịch tư vấn không hợp lệ";
        public const string NOT_FOUND_ONLINE = "Không tìm thấy đặt lịch trực tuyến";
        public const string NOT_FOUND_OFFLINE = "Không tìm thấy lịch tư vấn";
        public const string NOT_FOUND_REGISTERATTEND = "Không tìm thấy vé";
        public const string REQUIRED_ONE_ATLEAST = "Cần ít nhất một trong các trường booking";
        public const string REQUIRED_ONE = "Chỉ được assign một booking";

        public const string REQUIRED_ATLEAST_ONE = "Cần ít nhất một trong các trường booking";
        public const string PRICE_SELECTED_INVALID = "Giá được chọn không hợp lệ";
        public const string SERVICETYPE_CANCELED = "Buổi tư vấn đã bị hủy, không thể thanh toán";
        public const string NOT_PENDING_TO_PAY1ST = "Booking không ở trạng thái cho phép thanh toán lần 1";
        public const string NOT_PAID1ST_OR_PAID2ND = "Booking chưa thanh toán lần 1 hoặc đã thanh toán đủ";
        public const string BOOKING_NO_PACKAGE = "Booking chưa chọn gói tư vấn";
        public const string PRICE_NOT_CHOSEN = "Chưa chọn giá gói tư vấn";
        public const string PRICE_CHOSEN_SUCCES = "Chọn giá gói tư vấn thành công";
        public const string CHOOSE_PRICE_FOR_PENDING_ONLY = "Chỉ có thể chọn giá cho booking ở trạng thái Pending";
        public const string ALREADY_CREATE_BOOKING = "Vui lòng tạo Order cho tư vấn trước đó trước khi tạo tư vấn mới";
        public const string TIME_PASSED = "Không thể đặt lịch các ngày đã qua thời điểm hiện tại";
    }

    public class ResponseMessageConstrantsPackage
    {
        public const string PACKAGE_FOUND = "Tìm danh sách các gói tư vấn thành công";
        public const string ADDED_PACKAGE = "Chọn gói tư vấn thành công";
        public const string PACKAGE_NOT_FOUND = "Không tìm thấy gói tư vấn";
        public const string PACKAGE_EXISTED = "Bạn đã chọn gói tư vấn cho buổi tư vấn này. Nếu bạn muốn đặt gói mới, hãy hủy gói cũ";
        public const string REMOVED_PACKAGE = "Hủy gói tư vấn thành công";
        public const string PRICE_CHOSEN_SUCCESS = "Chọn giá thành công";
    }

    public class ResponseMessageConstrantsCompatibility
    {
        public const string DESTINY_NOT_FOUND = "Không tìm thấy cung mệnh. Vui lòng kiểm tra lại!";
        public const string INCORRECT_TOTAL_COLOR_RATIO = "Tổng tỉ lệ màu không đúng. Vui lòng kiểm tra lại!";
        public const string VERY_HIGH = "Rất hợp phong thủy, lý tưởng!.";
        public const string HIGH = "Hợp tốt, có thể sử dụng.";
        public const string MEDIUM = "Hợp trung bình, có thể chấp nhận.";
        public const string LOW = "Hợp mức thấp, có thể cải thiện thêm.";
        public const string VERY_LOW = "Rất không hợp, cần xem xét lại phong thủy.";
        public const string DESTINY_INVALID = "Mệnh không hợp lệ ";
    }

    public class ResponseMessageConstrantsKoiPond
    {
        public const string KOIPOND_NOT_FOUND = "Không tìm thấy hồ cá!";
        public const string KOIPOND_FOUND = "Lấy danh sách hồ cá thành công!";
        public const string KOIPOND_DESTINY_FOUND = "Lấy danh sách hồ cá phù hợp với mệnh thành công!";
        public const string KOIPOND_CREATED = "Tạo hồ cá thành công!";
        public const string KOIPOND_CREATE_FAILED = "Tạo hồ cá thất bại!";
        public const string KOIPOND_UPDATE_FAILED = "Cập nhật hồ cá thất bại!";
        public const string KOIPOND_UPDATED = "Cập nhật hồ cá thành công!";
        public const string KOIPOND_DELETED = "Xóa hồ cá thành công!";
        public const string KOIPOND_INVALID = "Dữ liệu hồ cá không hợp lệ";
        public const string SHAPE_NOT_FOUND = "Không tìm thấy hình dạng hồ cá!";
        public const string SHAPE_FOUND = "Tìm thấy danh sách hình dạng hồ cá!";

        public class ResponseMessageConstrantsKoiVariety
        {
            public const string KOIVARIETY_NOT_FOUND = "Không tìm thấy cá Koi!";
            public const string KOIVARIETY_FOUND = "Tìm thấy cá Koi thành công!";
            public const string KOIVARIETY_INFO_FOUND = "Lấy thông tin thành công ";
            public const string KOIVARIETY_COLOR_INFO_NOT_FOUND = "không có thông tin màu sắc ";
            public const string NO_MATCHES_KOIVARIETY = "Không tìm thấy Koi Variety phù hợp với mệnh của bạn. Hãy tham khảo tất cả loại cá chúng tôi c";
            public const string LOW_MATCHES_KOIVARIETY = "Chỉ tìm thấy Koi Variety có độ tương hợp thấp với mệnh của bạn";
            public const string GET_MATCHES_KOIVARIETY = "Lấy danh sách Koi Variety phù hợp với mệnh của bạn thành công";
            public const string CREATE_KOIVARIETY_FAILED = "Tạo cá Koi thất bại!";
            public const string CREATE_KOIVARIETY_SUCCESS = "Tạo cá Koi thành công!";
            public const string UPDATE_KOIVARIETY_FAILED = "Cập nhật cá Koi thất bại!";
            public const string UPDATE_KOIVARIETY_SUCCESS = "Cập nhật cá Koi thành công!";
            public const string DELETE_KOIVARIETY_SUCCESS = "Xóa cá Koi thành công!";
            public const string COLOR_NOT_FOUND = "Không tìm thấy màu sắc!";
            public const string COLOR_FOUND = "Lấy danh sách màu sắc thành công!";
            public const string CREATE_COLOR_FAILED = "Tạo màu sắc thất bại!";
            public const string CREATE_COLOR_SUCCESS = "Tạo màu sắc thành công!";
            public const string UPDATE_COLOR_SUCCESS = "Cập nhật màu sắc thành công!";
            public const string UPDATE_COLOR_FAILED = "Cập nhật màu sắc thất bại!";
            public const string DELETE_COLOR_SUCCESS = "Xóa màu sắc thành công!";
            public const string COLOR_INPUT_REQUIRED = "Không có màu nào được chọn!";
            public const string ELEMENT_COMPATIBLE_NOT_FOUND = "Không tìm thấy mệnh phù hợp với bất kỳ màu nào được chọn!";
            public const string INVALID_ELEMENT_FOR_COLORS = "Không thể xác định mệnh phù hợp cho các màu đã chọn!";

        }

        public class ResponseMessageConstrantsMasterSchedule
        {
            public const string MASTERSCHEDULE_NOT_FOUND = "Không tìm thấy thời gian biểu của Master!";
            public const string MASTERSCHEDULE_FOUND = "Lấy danh sách thời gian biểu của Master thành công!";
            public const string MASTERSCHEDULE_EXISTED_SLOT = "Thời gian biểu đã tồn tại!";
        }

        public class ResponseMessageConstrantsMaster
        {
            public const string MASTER_NOT_FOUND = "Không tìm thấy Master!";
            public const string MASTER_INFO_NOT_FOUND = "Không tìm thấy thông tin Master!";
            public const string MASTER_FOUND = "Lấy danh sách Master thành công!";
            public const string EXISTING_SCHEDULE = "Thời gian biểu của Master đã được đặt, vui lòng chọn thời gian khác!";
        }

        public class ResponseMessageConstrantsRegisterAttend
        {
            public const string REGISTERATTEND_NOT_FOUND = "Không tìm thấy vé dự sự kiện!";
            public const string INVALID_TICKET_NUMBER = "Số lượng vé phải lớn hơn 0!";
            public const string REGISTERATTEND_FOUND = "Lấy danh sách vé dự sự kiện thành công!";
            public const string REGISTERATTEND_CREATED_SUCCESS = "Tạo vé cho buổi sự kiện thành công!";
            public const string REGISTERATTEND_UPDATED_SUCCESS = "Cập nhật số lượng vé thành công!";
            public const string TICKET_NOT_PAID = "Vui lòng cập nhật số lượng vé. Số vé bạn chưa thanh toán cho workshop này là ";
            public const string PENDING_NOT_FOUND = "Không tìm thấy vé chưa thanh toán";
        }

        public class ResponseMessageConstrantsTransaction
        {
            public const string TRANSACTION_CREATED_SUCCESS = "Tạo giao dịch thành công!";
            public const string TRANSACTION_NOT_FOUND = "Không tìm thấy giao dịch!";
            public const string TRANSACTION_FOUND = "Lấy danh sách giao dịch thành công!";
        }

        public class ResponseMessageConstrantsWorkshop
        {
            public const string WORKSHOP_CREATED_SUCCESS = "Tạo sự kiện thành công!";
            public const string WORKSHOP_UPDATED_SUCCESS = "Cập nhật sự kiện thành công!";
            public const string WORKSHOP_DELETED_SUCCESS = "Xóa sự kiện thành công!";
            public const string WORKSHOP_FOUND = "Lấy danh sách sự kiện thành công!";
            public const string WORKSHOP_NOT_FOUND = "Không tìm thấy sự kiện!";
            public const string WORKSHOP_PENDING_NOT_FOUND = "Không tìm thấy buổi hội thảo đang chờ được cập nhật";
            public const string WORKSHOP_APPROVED = "Buổi hội thảo được phê duyệt";
            public const string WORKSHOP_REJECTED = "Buổi hội thảo bị từ chối";
            public const string WORKSHOP_INFO_FOUND = "Lấy thông tin buổi hội thảo thành công";
            public const string WORKSHOP_INFO_INVALID = "Dữ liệu sự kiện không hợp lệ";
            public const string NOTFOUND_MASTERID_CORRESPONDING_TO_ACCOUNT = "Không tìm thấy MasterId tương ứng với tài khoản";
            public const string REGISTER_NOT_FOUND = "Không tìm thấy thông tin vé";
            public const string CHECK_IN_SUCCESS = "Check-in thành công";
            public const string WORKSHOP_DUPLICATE_LOCATION_DATE_SAME_MASTER = "Bạn đã có một hội thảo với cùng địa điểm và ngày bắt đầu.";
            public const string WORKSHOP_DUPLICATE_LOCATION_DATE_OTHER_MASTER = "Đã có một hội thảo khác được tổ chức tại địa điểm và thời gian này.";
            public const string WORKSHOP_MINIMUM_HOURS_DIFFERENCE = "Khoảng cách giữa hai hội thảo ở hai địa điểm khác nhau phải tối thiểu 5 giờ.";
            public const string WORKSHOP_DELETE_NOT_ALLOWED = "Bạn không có quyền xóa workshop này.";
            public const string WORKSHOP_UPDATE_NOT_ALLOWED = "Bạn không có quyền cập nhật workshop này.";
            public const string ALREADY_STARTED = "Workshop đã bắt đầu, không thể đăng ký hay chỉnh sửa.";
            public const string CAPACITY_LEFT = "Vé còn trống cho workshop này là ";
        }

        public class ResponseMessageConstrantsCourse
        {
            public const string COURSE_CREATED_SUCCESS = "Tạo khóa học thành công!";
            public const string COURSE_UPDATED_SUCCESS = "Cập nhật khóa học thành công!";
            public const string COURSE_DELETED_SUCCESS = "Xóa khóa học thành công!";
            public const string COURSE_FOUND = "Lấy danh sách khóa học thành công!";
            public const string COURSE_NOT_FOUND = "Không tìm thấy khóa học!";
            public const string COURSE_INFO_FOUND = "Lấy thông tin khóa học thành công";
            public const string COURSE_INFO_INVALID = "Dữ liệu khóa học không hợp lệ";
            public const string COURSE_UPDATE_NOT_ALLOWED = "Bạn không có quyền cập nhật khóa học này.";
            public const string COURSE_DELETE_NOT_ALLOWED = "Bạn không có quyền xóa khóa học này.";
            public const string NOTFOUND_ACCOUNTID_CORRESPONDING_TO_ACCOUNT = "Không tìm thấy AccountId tương ứng với tài khoản";
            public const string COURSE_UPDATED_FAILED = "Cập nhật khóa học thất bại!";
            public const string COURSE_CREATED_FAILED = "Tạo khóa học thất bại!";
            public const string PROCEED_TO_QUIZ_SUCCESS = "Bạn đã hoàn thành xong các chương, hãy làm quiz!";
            public const string PAID_COURSES_NOT_FOUND = "Chưa có khóa học nào được mua!";
            public const string PAID_COURSES_FOUND = "Lấy danh sách khóa học đã mua thành công!";
        }

        public class ResponseMessageConstrantsCategory
        {
            public const string CATEGORY_CREATED_SUCCESS = "Tạo danh mục thành công!";
            public const string CATEGORY_UPDATED_SUCCESS = "Cập nhật danh mục thành công!";
            public const string CATEGORY_FOUND = "Lấy danh sách danh mục thành công!";
            public const string CATEGORY_NOT_FOUND = "Không tìm thấy danh sách danh mục!";
            public const string CATEGORY_ALREADY_EXIST = "Tên danh mục đã tồn tại!";
        }

        //Image
        public class ResponseMessageImage
        {
            public const string INVALID_IMAGE = "Hình ảnh không hợp lệ. ";
            public const string INVALID_SIZE = "Kích thước hình ảnh không hợp lệ. ";
            public const string INVALID_FORMAT = "Định dạng hình ảnh không hợp lệ. ";
            public const string INVALID_URL = "Đường dẫn hình ảnh không hợp lệ. ";
        }

        public class ResponseMessageConstrantsChapter
        {
            public const string CHAPTER_CREATED_SUCCESS = "Tạo chương học thành công!";
            public const string CHAPTER_UPDATED_SUCCESS = "Cập nhật chương học thành công!";
            public const string CHAPTER_DELETED_SUCCESS = "Xóa chương học thành công!";
            public const string CHAPTER_FOUND = "Lấy danh sách chương học thành công!";
            public const string CHAPTER_NOT_FOUND = "Không tìm thấy chương học!";
            public const string CHAPTER_INFO_FOUND = "Lấy thông tin chương học thành công";
            public const string CHAPTER_INFO_INVALID = "Dữ liệu chương học không hợp lệ";
            public const string COURSE_ID_REQUIRED = "Không tìm thấy Id khóa học";
            public const string ENROLL_CHAPTER_CREATE_FAILED = "Tạo chương học thất bại!";
            public const string CHAPTER_UPDATED_PROGRESS_SUCCESS = "Cập nhật tiến độ chương học thành công!";
            public const string CHAPTER_ALREADY_COMPLETED = "Chương học đã hoàn thành!";
        }

        public class ResponseMessageConstrantQuiz
        {
            public const string QUIZ_CREATED_SUCCESS = "Tạo bài kiểm tra thành công!";
            public const string QUIZ_UPDATED_SUCCESS = "Cập nhật bài kiểm tra thành công!";
            public const string QUIZ_DELETED_SUCCESS = "Xóa bài kiểm tra thành công!";
            public const string QUIZ_FOUND = "Lấy danh sách bài kiểm tra thành công!";
            public const string QUIZ_NOT_FOUND = "Không tìm thấy bài kiểm tra!";
            public const string QUIZ_INFO_FOUND = "Lấy thông tin bài kiểm tra thành công";
            public const string QUIZ_INFO_INVALID = "Dữ liệu bài kiểm tra không hợp lệ";
            public const string COURSE_ID_REQUIRED = "Không tìm thấy Id khóa học";
            public const string QUIZ_CREATE_FAILED = "Tạo bài kiểm tra thất bại!";
            public const string QUIZ_CREATE_SUCCESS = "Tạo bài kiểm tra thành công!";
            public const string QUIZ_DELETE_SUCCESS = "Xóa bài kiểm tra thành công!";
            public const string QUIZ_UPDATE_SUCCESS = "Cập nhật bài kiểm tra thành công!";
            public const string QUIZ_UPDATE_FAILED = "Cập nhật bài kiểm tra thất bại!";
        }

        public class ResponseMessageConstrantsQuestion
        {
            public const string QUESTION_CREATED_SUCCESS = "Tạo câu hỏi thành công!";
            public const string QUESTION_UPDATED_SUCCESS = "Cập nhật câu hỏi thành công!";
            public const string QUESTION_DELETED_SUCCESS = "Xóa câu hỏi thành công!";
            public const string QUESTION_FOUND = "Lấy danh sách câu hỏi thành công!";
            public const string QUESTION_NOT_FOUND = "Không tìm thấy câu hỏi!";
            public const string QUESTION_INFO_FOUND = "Lấy thông tin câu hỏi thành công";
            public const string QUESTION_INFO_INVALID = "Dữ liệu câu hỏi không hợp lệ";
            public const string QUIZ_ID_REQUIRED = "Không tìm thấy Id bài kiểm tra";
            public const string QUESTION_CREATE_FAILED = "Tạo câu hỏi thất bại!";
            public const string QUESTION_UPDATED_FAILED = "Cập nhật câu hỏi thất bại!";
            public const string QUESTION_DELETED_FAILED = "Xóa câu hỏi thất bại!";
            public const string QUESTIONS_NOT_FOUND = "Không tìm thấy câu hỏi!";
            public const string QUESTIONS_FOUND = "Lấy danh sách câu hỏi thành công!";
        }

        public class ResponseMessageConstrantsAnswer
        {
            public const string ANSWER_CREATED_SUCCESS = "Tạo câu trả lời thành công!";
            public const string ANSWER_UPDATED_SUCCESS = "Cập nhật câu trả lời thành công!";
            public const string ANSWER_DELETED_SUCCESS = "Xóa câu trả lời thành công!";
            public const string ANSWER_FOUND = "Lấy danh sách câu trả lời thành công!";
            public const string ANSWER_NOT_FOUND = "Không tìm thấy câu trả lời!";
            public const string ANSWER_INFO_FOUND = "Lấy thông tin câu trả lời thành công";
            public const string ANSWER_INFO_INVALID = "Dữ liệu câu trả lời không hợp lệ";
            public const string QUESTION_ID_REQUIRED = "Không tìm thấy Id câu hỏi";
            public const string ANSWER_CREATE_FAILED = "Tạo câu trả lời thất bại!";
            public const string ANSWER_UPDATED_FAILED = "Cập nhật câu trả lời thất bại!";
            public const string ANSWER_DELETED_FAILED = "Xóa câu trả lời thất bại!";
            public const string INVALID_ANSWER = "Câu trả lời không hợp lệ";
        }

        public class ResponseMessageConstrantsRegisterCourse
        {
            public const string REGISTER_COURSE_CREATED_SUCCESS = "Tạo đăng ký khóa học thành công!";
            public const string REGISTER_COURSE_UPDATED_SUCCESS = "Cập nhật khóa học thành công!";
            public const string REGISTER_COURSE_DELETED_SUCCESS = "Xóa khóa học thành công!";
            public const string REGISTER_COURSE_FOUND = "Lấy danh sách khóa học thành công!";
            public const string REGISTER_COURSE_NOT_FOUND = "Không tìm thấy khóa học!";
            public const string REGISTER_COURSE_INFO_FOUND = "Lấy thông tin khóa học thành công";
            public const string REGISTER_COURSE_INFO_INVALID = "Dữ liệu khóa học không hợp lệ";
            public const string REGISTER_COURSE_CREATE_FAILED = "Tạo đăng ký khóa học thất bại!";
            public const string REGISTER_COURSE_UPDATE_FAILED = "Cập nhật đăng ký khóa học thất bại!";
            public const string REGISTER_COURSE_DELETE_FAILED = "Xóa đăng ký khóa học thất bại!";
        }
        // Contract Service
        public static class ResponseMessageConstrantsContract
        {
            public const string NOT_FOUND = "Không tìm thấy hợp đồng";
            public const string FOUND = "Tìm thấy hợp đồng";
            public const string CREATED_SUCCESS = "Tạo hợp đồng thành công";
            public const string SENT_SUCCESS = "Gửi hợp đồng thành công";
            public const string OTP_SENT_SUCCESS = "Gửi mã OTP thành công";
            public const string OTP_INVALID = "Mã OTP không đúng hoặc đã hết hạn";
            public const string VERIFY_SUCCESS = "Xác thực hợp đồng thành công";
            public const string CANCEL_SUCCESS = "Hủy hợp đồng thành công";
            public const string CONFIRM_SUCCESS = "Xác nhận hợp đồng thành công";
            public const string CHECK_STATUS= "Hợp đồng này không ở trạng thái chờ xác nhận";
            public const string CONTRACT_INFORMATION_SUCCESS = "Lấy thông tin hợp đồng thành công";
            public const string SEND_OTP_SUCCESS = "Gửi OTP cho hợp đồng thành công";
            public const string OTP_EXPIRED = "Mã OTP đã hết hạn";
            public const string VERIFY_OTP_SUCCESS = "Xác thực OTP thành công";
            public const string VERIFY_OTP_FAILED = "Xác thực OTP thất bại";
            public const string WAIT_TO_RESEND_OTP = "Vui lòng đợi 30s để gửi lại mã OTP";
            public const string RESEND_OTP_SUCCESS = "Gửi lại mã OTP thành công";
        }

        // 
        public static class ResponseMessageConstrantsForImport
        {
            public const string NOT_FOUND = "Không tìm thấy file";
            public const string NO_DATA_TO_UPLOAD = "Không có dữ liệu để upload";
            public const string UPLOAD_SUCCESS = "Upload thành công";
            public const string NO_QUES_FOR_ANS = "Không thể thêm câu trả lời khi chưa có câu hỏi";
            public const string NO_QUIZ_FOR_QUES= "Không thể thêm câu hỏi khi chưa có bài kiểm tra";
            public const string EXISTED_QUIZ_TITLE= "Tiêu đề của bài kiểm tra đã tồn tại";
            public const string FILE_INVALID= "File phải có định dạng .xls hoặc .xlsx";
        }
    }
}

