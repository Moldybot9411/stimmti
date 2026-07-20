/* eslint-disable */
/* tslint:disable */
// @ts-nocheck
/*
 * ---------------------------------------------------------------
 * ## THIS FILE WAS GENERATED VIA SWAGGER-TYPESCRIPT-API        ##
 * ##                                                           ##
 * ## AUTHOR: acacode                                           ##
 * ## SOURCE: https://github.com/acacode/swagger-typescript-api ##
 * ---------------------------------------------------------------
 */

export enum QuestionTypeEnum {
  SingleChoice = "SingleChoice",
  MultipleChoice = "MultipleChoice",
  WordCloud = "WordCloud",
  FreeText = "FreeText",
  NumberScale = "NumberScale",
}

export enum HatProfileEnum {
  Hat01 = "Hat01",
  Hat02 = "Hat02",
  Hat03 = "Hat03",
  Hat04 = "Hat04",
  Hat05 = "Hat05",
}

export enum FaceProfileEnum {
  Face01 = "Face01",
  Face02 = "Face02",
  Face03 = "Face03",
  Face04 = "Face04",
  Face05 = "Face05",
}

export enum ColorProfileEnum {
  Green = "Green",
  Blue = "Blue",
  Red = "Red",
  Purple = "Purple",
  Yellow = "Yellow",
}

export enum BodyProfileEnum {
  Body01 = "Body01",
  Body02 = "Body02",
  Body03 = "Body03",
  Body04 = "Body04",
  Body05 = "Body05",
}

export interface AnswerDisplayDto {
  choiceResults: ChoiceResultDto[];
  freeTextResults: FreeTextResultDto[];
  wordCloudResults: WordCloudResultDto[];
  numberResults: NumberResultDto[];
  /** @format int32 */
  totalParticipantsAnswered?: number;
}

export interface AnswerOptionDto {
  /** @format uuid */
  id: string;
  description: string | null;
}

export interface ChoiceResultDto {
  answerOption: AnswerOptionDto;
  /** @format int32 */
  count: number;
}

export interface CreateFolderDto {
  /** @maxLength 255 */
  name?: string | null;
}

export interface CreateFolderResponseDto {
  /** @format uuid */
  folderId: string;
}

export interface CreateSessionDto {
  /** @maxLength 255 */
  name: string | null;
  description?: string | null;
  /** @format uuid */
  surveyId: string;
}

export interface CreateSessionResponseDto {
  name: string | null;
  description?: string | null;
  roomCode: string | null;
}

export interface CreateSurveyDto {
  /** @maxLength 255 */
  title?: string | null;
  /** @maxLength 2048 */
  description?: string | null;
  /** @format uuid */
  folderId?: string | null;
}

export interface CreateSurveyResponseDto {
  /** @format uuid */
  surveyId: string;
  /** @format uuid */
  folderId?: string | null;
}

export interface DisplaynameCheckDto {
  /**
   * @maxLength 20
   * @pattern ^[A-Za-z0-9]+$
   */
  displayName: string | null;
}

export interface FreeTextResultDto {
  /** @format uuid */
  id: string;
  text: string | null;
}

export interface GetFolderResponseDto {
  /** @format uuid */
  folderId: string;
  name: string | null;
  surveys: GetSurveyResponseDto[] | null;
}

export interface GetStatisticsDto {
  /** @format int32 */
  surveyCount: number;
  /** @format int32 */
  surveyDelta: number;
  /** @format int32 */
  sessionCount: number;
  /** @format int32 */
  sessionDelta: number;
  /** @format int32 */
  participantCount: number;
  /** @format int32 */
  participantDelta: number;
  surveyStatistics?: SurveyStatisticsDto[] | null;
  sessionStatistics?: SessionStatisticsDto[] | null;
}

export interface GetSurveyResponseDto {
  /** @format uuid */
  surveyId: string;
  title: string | null;
  description?: string | null;
  /** @format uuid */
  folderId?: string | null;
}

export interface IdentityError {
  code?: string | null;
  description?: string | null;
}

export interface NumberResultDto {
  /** @format int32 */
  value: number;
  /** @format int32 */
  count: number;
}

export interface ProblemDetails {
  type?: string | null;
  title?: string | null;
  /** @format int32 */
  status?: number | null;
  detail?: string | null;
  instance?: string | null;
  [key: string]: any;
}

export interface QuestionDto {
  questionTemplateDto: QuestionTemplateDto;
  answerDisplayDto: AnswerDisplayDto;
}

export interface QuestionTemplateDto {
  name: string | null;
  description?: string | null;
  questionType: QuestionTypeEnum;
  answerOptions?: AnswerOptionDto[] | null;
  /** @format int32 */
  minValue?: number | null;
  /** @format int32 */
  maxValue?: number | null;
  /** @format int32 */
  wordCloudMaxWords?: number | null;
}

export interface SessionResultDto {
  name: string | null;
  description?: string | null;
  /** @format date-time */
  openedAt: string;
  questions: QuestionDto[];
}

export interface SessionStatisticsDto {
  name: string | null;
  /** @format int32 */
  participantCount: number;
  /** @format date-time */
  openedAt: string;
}

export interface SurveyStatisticsDto {
  name: string | null;
  /** @format int32 */
  numSessions: number;
}

export interface UpdateFolderDto {
  /** @maxLength 255 */
  name?: string | null;
}

export interface UpdateSurveyDto {
  /** @maxLength 255 */
  title?: string | null;
  /** @maxLength 2048 */
  description?: string | null;
  /** @format uuid */
  folderId?: string | null;
  removeFromFolder?: boolean | null;
}

export interface UserAuthDto {
  /** @format uuid */
  id?: string;
  username: string | null;
  displayName: string | null;
  profilePictureUrl?: string | null;
}

export interface UserLoginDto {
  password: string | null;
  username: string | null;
  staySignedIn?: boolean;
}

export interface UserPasswordDto {
  /** @minLength 1 */
  oldPassword: string;
  /** @minLength 1 */
  newPassword: string;
}

export interface UserRegisterDto {
  username?: string | null;
  password?: string | null;
  email?: string | null;
}

export interface UserUsernameAvailabilityResponseDto {
  isAvailable?: boolean;
  message?: string | null;
}

export interface ValidationProblemDetails {
  type?: string | null;
  title?: string | null;
  /** @format int32 */
  status?: number | null;
  detail?: string | null;
  instance?: string | null;
  errors?: Record<string, string[]> | null;
  [key: string]: any;
}

export interface WordCloudResultDto {
  /** @minLength 1 */
  text: string;
  /** @format int32 */
  count: number;
}

import type {
  AxiosInstance,
  AxiosRequestConfig,
  AxiosResponse,
  HeadersDefaults,
  ResponseType,
} from "axios";
import axios from "axios";

export type QueryParamsType = Record<string | number, any>;

export interface FullRequestParams
  extends Omit<AxiosRequestConfig, "data" | "params" | "url" | "responseType"> {
  /** set parameter to `true` for call `securityWorker` for this request */
  secure?: boolean;
  /** request path */
  path: string;
  /** content type of request body */
  type?: ContentType;
  /** query params */
  query?: QueryParamsType;
  /** format of response (i.e. response.json() -> format: "json") */
  format?: ResponseType;
  /** request body */
  body?: unknown;
}

export type RequestParams = Omit<
  FullRequestParams,
  "body" | "method" | "query" | "path"
>;

export interface ApiConfig<SecurityDataType = unknown>
  extends Omit<AxiosRequestConfig, "data" | "cancelToken"> {
  securityWorker?: (
    securityData: SecurityDataType | null,
  ) => Promise<AxiosRequestConfig | void> | AxiosRequestConfig | void;
  secure?: boolean;
  format?: ResponseType;
}

export enum ContentType {
  Json = "application/json",
  JsonApi = "application/vnd.api+json",
  FormData = "multipart/form-data",
  UrlEncoded = "application/x-www-form-urlencoded",
  Text = "text/plain",
}

export class HttpClient<SecurityDataType = unknown> {
  public instance: AxiosInstance;
  private securityData: SecurityDataType | null = null;
  private securityWorker?: ApiConfig<SecurityDataType>["securityWorker"];
  private secure?: boolean;
  private format?: ResponseType;

  constructor({
    securityWorker,
    secure,
    format,
    ...axiosConfig
  }: ApiConfig<SecurityDataType> = {}) {
    this.instance = axios.create({
      ...axiosConfig,
      baseURL: axiosConfig.baseURL || "",
    });
    this.secure = secure;
    this.format = format;
    this.securityWorker = securityWorker;
  }

  public setSecurityData = (data: SecurityDataType | null) => {
    this.securityData = data;
  };

  protected mergeRequestParams(
    params1: AxiosRequestConfig,
    params2?: AxiosRequestConfig,
  ): AxiosRequestConfig {
    const method = params1.method || (params2 && params2.method);

    return {
      ...this.instance.defaults,
      ...params1,
      ...(params2 || {}),
      headers: {
        ...((method &&
          this.instance.defaults.headers[
            method.toLowerCase() as keyof HeadersDefaults
          ]) ||
          {}),
        ...(params1.headers || {}),
        ...((params2 && params2.headers) || {}),
      },
    };
  }

  protected stringifyFormItem(formItem: unknown) {
    if (typeof formItem === "object" && formItem !== null) {
      return JSON.stringify(formItem);
    } else {
      return `${formItem}`;
    }
  }

  protected createFormData(input: Record<string, unknown>): FormData {
    if (input instanceof FormData) {
      return input;
    }
    return Object.keys(input || {}).reduce((formData, key) => {
      const property = input[key];
      const propertyContent: any[] =
        property instanceof Array ? property : [property];

      for (const formItem of propertyContent) {
        const isFileType = formItem instanceof Blob || formItem instanceof File;
        formData.append(
          key,
          isFileType ? formItem : this.stringifyFormItem(formItem),
        );
      }

      return formData;
    }, new FormData());
  }

  public request = async <T = any, _E = any>({
    secure,
    path,
    type,
    query,
    format,
    body,
    ...params
  }: FullRequestParams): Promise<AxiosResponse<T>> => {
    const secureParams =
      ((typeof secure === "boolean" ? secure : this.secure) &&
        this.securityWorker &&
        (await this.securityWorker(this.securityData))) ||
      {};
    const requestParams = this.mergeRequestParams(params, secureParams);
    const responseFormat = format || this.format || undefined;

    if (
      type === ContentType.FormData &&
      body &&
      body !== null &&
      typeof body === "object"
    ) {
      body = this.createFormData(body as Record<string, unknown>);
    }

    if (
      type === ContentType.Text &&
      body &&
      body !== null &&
      typeof body !== "string"
    ) {
      body = JSON.stringify(body);
    }

    return this.instance.request({
      ...requestParams,
      headers: {
        ...(requestParams.headers || {}),
        ...(type ? { "Content-Type": type } : {}),
      },
      params: query,
      responseType: responseFormat,
      data: body,
      url: path,
    });
  };
}

/**
 * @title Backend
 * @version 1.0
 */
export class Api<
  SecurityDataType extends unknown,
> extends HttpClient<SecurityDataType> {
  api = {
    /**
     * No description
     *
     * @tags Session
     * @name V1SessionCreateSessionCreate
     * @request POST:/api/v1/Session/createSession
     */
    v1SessionCreateSessionCreate: (
      data: CreateSessionDto,
      params: RequestParams = {},
    ) =>
      this.request<CreateSessionResponseDto, ProblemDetails>({
        path: `/api/v1/Session/createSession`,
        method: "POST",
        body: data,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Session
     * @name V1SessionCheckSessionList
     * @request GET:/api/v1/Session/checkSession
     */
    v1SessionCheckSessionList: (
      query?: {
        roomCode?: string;
      },
      params: RequestParams = {},
    ) =>
      this.request<void, ProblemDetails>({
        path: `/api/v1/Session/checkSession`,
        method: "GET",
        query: query,
        ...params,
      }),

    /**
     * No description
     *
     * @tags Session
     * @name V1SessionSessionList
     * @request GET:/api/v1/Session/session
     */
    v1SessionSessionList: (
      query?: {
        /** @format uuid */
        sessionId?: string;
      },
      params: RequestParams = {},
    ) =>
      this.request<SessionResultDto, ProblemDetails>({
        path: `/api/v1/Session/session`,
        method: "GET",
        query: query,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Statistics
     * @name V1StatisticsStatisticsList
     * @request GET:/api/v1/Statistics/statistics
     */
    v1StatisticsStatisticsList: (
      query?: {
        /**
         * @format int32
         * @default 10
         */
        pageSize?: number;
        /**
         * @format int32
         * @default 1
         */
        currentSurveyPage?: number;
        /**
         * @format int32
         * @default 1
         */
        currentSessionPage?: number;
      },
      params: RequestParams = {},
    ) =>
      this.request<GetStatisticsDto, ProblemDetails>({
        path: `/api/v1/Statistics/statistics`,
        method: "GET",
        query: query,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Survey
     * @name V1SurveyCreate
     * @request POST:/api/v1/Survey
     */
    v1SurveyCreate: (data: CreateSurveyDto, params: RequestParams = {}) =>
      this.request<CreateSurveyResponseDto, ProblemDetails>({
        path: `/api/v1/Survey`,
        method: "POST",
        body: data,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Survey
     * @name V1SurveyList
     * @request GET:/api/v1/Survey
     */
    v1SurveyList: (params: RequestParams = {}) =>
      this.request<GetSurveyResponseDto[], ProblemDetails>({
        path: `/api/v1/Survey`,
        method: "GET",
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Survey
     * @name V1SurveyPartialUpdate
     * @request PATCH:/api/v1/Survey/{surveyId}
     */
    v1SurveyPartialUpdate: (
      surveyId: string,
      data: UpdateSurveyDto,
      params: RequestParams = {},
    ) =>
      this.request<any, ProblemDetails>({
        path: `/api/v1/Survey/${surveyId}`,
        method: "PATCH",
        body: data,
        type: ContentType.Json,
        ...params,
      }),

    /**
     * No description
     *
     * @tags Survey
     * @name V1SurveyFoldersCreate
     * @request POST:/api/v1/Survey/folders
     */
    v1SurveyFoldersCreate: (
      data: CreateFolderDto,
      params: RequestParams = {},
    ) =>
      this.request<CreateFolderResponseDto, ProblemDetails>({
        path: `/api/v1/Survey/folders`,
        method: "POST",
        body: data,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Survey
     * @name V1SurveyFoldersList
     * @request GET:/api/v1/Survey/folders
     */
    v1SurveyFoldersList: (params: RequestParams = {}) =>
      this.request<GetFolderResponseDto[], ProblemDetails>({
        path: `/api/v1/Survey/folders`,
        method: "GET",
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Survey
     * @name V1SurveyFoldersPartialUpdate
     * @request PATCH:/api/v1/Survey/folders/{folderId}
     */
    v1SurveyFoldersPartialUpdate: (
      folderId: string,
      data: UpdateFolderDto,
      params: RequestParams = {},
    ) =>
      this.request<any, ProblemDetails>({
        path: `/api/v1/Survey/folders/${folderId}`,
        method: "PATCH",
        body: data,
        type: ContentType.Json,
        ...params,
      }),

    /**
     * No description
     *
     * @tags User
     * @name V1UserRegisterCreate
     * @request POST:/api/v1/User/register
     */
    v1UserRegisterCreate: (data: UserRegisterDto, params: RequestParams = {}) =>
      this.request<void, IdentityError[]>({
        path: `/api/v1/User/register`,
        method: "POST",
        body: data,
        type: ContentType.Json,
        ...params,
      }),

    /**
     * No description
     *
     * @tags User
     * @name V1UserLoginCreate
     * @request POST:/api/v1/User/login
     */
    v1UserLoginCreate: (data: UserLoginDto, params: RequestParams = {}) =>
      this.request<void, any>({
        path: `/api/v1/User/login`,
        method: "POST",
        body: data,
        type: ContentType.Json,
        ...params,
      }),

    /**
     * No description
     *
     * @tags User
     * @name V1UserLogoutCreate
     * @request POST:/api/v1/User/logout
     */
    v1UserLogoutCreate: (params: RequestParams = {}) =>
      this.request<void, any>({
        path: `/api/v1/User/logout`,
        method: "POST",
        ...params,
      }),

    /**
     * No description
     *
     * @tags User
     * @name V1UserCheckUsernameList
     * @request GET:/api/v1/User/checkUsername
     */
    v1UserCheckUsernameList: (
      query: {
        /**
         * @maxLength 20
         * @pattern ^[A-Za-z0-9]+$
         */
        Username: string;
      },
      params: RequestParams = {},
    ) =>
      this.request<
        UserUsernameAvailabilityResponseDto,
        ValidationProblemDetails
      >({
        path: `/api/v1/User/checkUsername`,
        method: "GET",
        query: query,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags User
     * @name V1UserMeList
     * @request GET:/api/v1/User/me
     */
    v1UserMeList: (params: RequestParams = {}) =>
      this.request<UserAuthDto, ProblemDetails>({
        path: `/api/v1/User/me`,
        method: "GET",
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags User
     * @name V1UserDisplayNamePartialUpdate
     * @request PATCH:/api/v1/User/displayName
     */
    v1UserDisplayNamePartialUpdate: (
      data: DisplaynameCheckDto,
      params: RequestParams = {},
    ) =>
      this.request<string, ProblemDetails>({
        path: `/api/v1/User/displayName`,
        method: "PATCH",
        body: data,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags User
     * @name V1UserPasswordPartialUpdate
     * @request PATCH:/api/v1/User/password
     */
    v1UserPasswordPartialUpdate: (
      data: UserPasswordDto,
      params: RequestParams = {},
    ) =>
      this.request<void, ValidationProblemDetails>({
        path: `/api/v1/User/password`,
        method: "PATCH",
        body: data,
        type: ContentType.Json,
        ...params,
      }),

    /**
     * No description
     *
     * @tags User
     * @name V1UserProfilePictureCreate
     * @request POST:/api/v1/User/profilePicture
     */
    v1UserProfilePictureCreate: (
      data: {
        /** @format binary */
        file?: File;
      },
      params: RequestParams = {},
    ) =>
      this.request<string, ProblemDetails>({
        path: `/api/v1/User/profilePicture`,
        method: "POST",
        body: data,
        type: ContentType.FormData,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags User
     * @name V1UserDeleteUserDelete
     * @request DELETE:/api/v1/User/DeleteUser
     */
    v1UserDeleteUserDelete: (params: RequestParams = {}) =>
      this.request<void, IdentityError[] | ProblemDetails>({
        path: `/api/v1/User/DeleteUser`,
        method: "DELETE",
        ...params,
      }),
  };
}
